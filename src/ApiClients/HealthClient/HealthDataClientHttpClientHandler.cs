using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Clinician.ApiClients.HealthClient.Models;
using Clinician.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Clinician.ApiClients.HealthClient
{
    public class HealthDataClientHttpClientHandler : HttpClientHandler
    {
        private readonly IAccessTokenAccessor accessTokenAccessor;
        private readonly ILogger<HealthDataClientHttpClientHandler> logger;

        public HealthDataClientHttpClientHandler(IAccessTokenAccessor accessTokenAccessor, ILogger<HealthDataClientHttpClientHandler> logger)
        {
            this.accessTokenAccessor = accessTokenAccessor;
            this.logger = logger;
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

            if (this.logger.IsEnabled(LogLevel.Debug) && request.Content != null)
            {
                var requestString = await request.Content.ReadAsStringAsync();
                this.logger.LogDebug(requestString);
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                if (response.Content != null && response.Content.Headers.ContentType.MediaType.Equals("application/problem+json"))
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

                if (this.logger.IsEnabled(LogLevel.Error) && response.Content != null)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    this.logger.LogError(content);
                }

                response.EnsureSuccessStatusCode();
            }
            return response;

        }
    }
}