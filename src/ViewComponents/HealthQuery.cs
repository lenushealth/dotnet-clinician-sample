using System;
using System.Linq;
using System.Threading.Tasks;
using Clinician.ApiClients.HealthClient;
using Clinician.ApiClients.HealthClient.Models;
using Clinician.Models;
using Clinician.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clinician.ViewComponents
{
    public class HealthQuery : ViewComponent
    {
        private readonly IAgencySubjectQueryService agencySubjectQueryService;

        public HealthQuery(IAgencySubjectQueryService agencySubjectQueryService)
        {
            this.agencySubjectQueryService = agencySubjectQueryService;
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
            try
            {
                var response = await agencySubjectQueryService.QueryAsync(type, this.GetParametersFromRequest());
                return this.View(type.ToString(), response);
            }
            catch (ProblemDetailsException<ProblemDetails> exception)
            {
                return this.View("ProblemDetails", exception.ProblemDetails);
            }
            catch (Exception exception)
            {
                return this.View("Error", new ErrorViewModel()
                {
                    RequestId = this.HttpContext.TraceIdentifier,
                    Exception = exception
                });
            }
        }
    }
}
