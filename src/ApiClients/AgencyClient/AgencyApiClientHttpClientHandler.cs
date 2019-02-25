using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Clinician.ApiClients.HealthClient;
using Clinician.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Clinician.ApiClients.AgencyClient
{
    public class AgencyApiClientHttpClientHandler : DelegatingHandler
    {
        private readonly IAccessTokenAccessor accessTokenAccessor;

        public AgencyApiClientHttpClientHandler(IAccessTokenAccessor accessTokenAccessor)
        {
            this.accessTokenAccessor = accessTokenAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // See if the request has an authorize header
            var auth = request.Headers.Authorization;
            if (auth != null)
            {
                var token = await this.accessTokenAccessor.GetAccessTokenAsync().ConfigureAwait(false);
                request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, token);
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                if (response.Content.Headers.ContentType?.MediaType?.Equals("application/json+problem") ?? false)
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            throw new HealthDataClientException(
                                JsonConvert.DeserializeObject<ValidationProblemDetails>(errorResponse));
                        case HttpStatusCode.Unauthorized:
                        default:
                            throw new HealthDataClientException(
                                JsonConvert.DeserializeObject<ProblemDetails>(errorResponse));
                    }
                }

                response.EnsureSuccessStatusCode();
            }
            return response;

        }
    }
}