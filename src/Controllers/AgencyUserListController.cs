using System.Collections.Generic;
using System.Threading.Tasks;
using Clinician.Models;
using Clinician.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Clinician.Controllers
{
    [Authorize(Policy = "AsAgent")]
    [Route("agency/list")]
    public class AgencyUserListController : Controller
    {
        private readonly IAgencySubjectQueryService agencySubjectQueryService;

        public AgencyUserListController(IAgencySubjectQueryService agencySubjectQueryService)
        {
            this.agencySubjectQueryService = agencySubjectQueryService;
        }

        [Route("")]
        public async Task<IActionResult> Index([FromQuery] IEnumerable<SampleDataTypes> type, [FromQuery] AgencySubjectQueryParameters parameters)
        {
            var claimsResponse = await this.agencySubjectQueryService.GetAgencySubjectsAsync();

            var subjectList = new List<AgencySubjectViewModel>();

            foreach (var claimSet in claimsResponse)
            {
                var subject = new AgencySubjectViewModel
                {
                    Name = claimSet.Name,
                    GivenName = claimSet.GivenName,
                    FamilyName = claimSet.FamilyName,
                    Subject = claimSet.Subject,
                    HealthDataScopes = claimSet.HealthDataScopes
                };
                subjectList.Add(subject);
            }

            return this.View(subjectList);
        }
    }
}
