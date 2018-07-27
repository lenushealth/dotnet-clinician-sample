using System;
using System.Linq;
using System.Threading.Tasks;
using Clinician.ApiClients.HealthClient;
using Clinician.ApiClients.HealthClient.Models;
using Clinician.Models;
using Clinician.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Clinician.ViewComponents
{
    public class HealthQuery : ViewComponent
    {
        private readonly IAgencySubjectQueryService agencySubjectQueryService;
        private readonly ILogger<HealthQuery> logger;

        public HealthQuery(IAgencySubjectQueryService agencySubjectQueryService, ILogger<HealthQuery> logger)
        {
            this.agencySubjectQueryService = agencySubjectQueryService;
            this.logger = logger;
        }

        private AgencySubjectQueryParameters GetParametersFromRequest()
        {
            var parameters = new AgencySubjectQueryParameters();

            if (Guid.TryParse(Request.Query["subject"].FirstOrDefault(), out var subject))
            {
                parameters.Subject = subject;
            }

            if (int.TryParse(Request.Query["take"].FirstOrDefault(), out var take))
            {
                parameters.Take = take;
            }

            if (DateTimeOffset.TryParse(Request.Query["from"].FirstOrDefault(), out var from))
            {
                parameters.From = from;
            }

            if (DateTimeOffset.TryParse(Request.Query["to"].FirstOrDefault(), out var to))
            {
                parameters.To = to;
            }

            return parameters;
        }

        public async Task<IViewComponentResult> InvokeAsync(SampleDataTypes type)
        {
            var prms = this.GetParametersFromRequest();
            try
            {
                var response = await agencySubjectQueryService.QueryAsync(type, prms);
                return this.View(type.ToString(), response);
            }
            catch (ProblemDetailsException<ProblemDetails> exception)
            {
                logger.LogError(exception, $"Error invoking query for subject {prms.Subject}");
                return this.View("ProblemDetails", exception.ProblemDetails);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"Error invoking query for subject {prms.Subject}");
                return this.View("Error", new ErrorViewModel()
                {
                    RequestId = this.HttpContext.TraceIdentifier,
                    Exception = exception
                });
            }
        }
    }
}
