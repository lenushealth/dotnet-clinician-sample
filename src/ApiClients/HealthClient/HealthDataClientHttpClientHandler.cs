using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Clinician.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Clinician.ApiClients.HealthClient
{
    public class HealthDataClientHttpClientHandler : DelegatingHandler
    {
        private readonly IAccessTokenAccessor accessTokenAccessor;
        private readonly ILogger<HealthDataClientHttpClientHandler> logger;

        public HealthDataClientHttpClientHandler(IAccessTokenAccessor accessTokenAccessor, ILogger<HealthDataClientHttpClientHandler> logger)
        {
            this.accessTokenAccessor = accessTokenAccessor;
            this.logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var auth = request.Headers.Authorization;
            if (auth != null)
            {
                var token = await accessTokenAccessor.GetAccessTokenAsync().ConfigureAwait(false);
                request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, token);
            }

            if (logger.IsEnabled(LogLevel.Debug) && request.Content != null)
            {
                var requestString = await request.Content.ReadAsStringAsync();
                logger.LogDebug(requestString);
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                if (response.Content != null &&
                    response.Content.Headers.ContentType?.MediaType.Equals("application/problem+json") == true
                )
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();

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

                var content = await response.Content.ReadAsStringAsync();
                logger.LogError(content);
                response.EnsureSuccessStatusCode();
            }

            return response;
        }
    }
}