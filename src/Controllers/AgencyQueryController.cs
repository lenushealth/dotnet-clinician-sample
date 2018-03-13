using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinician.Models;
using Clinician.Services;
using Clinician.Services.Impl;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis.Semantics;

namespace Clinician.Controllers
{
    [Authorize(Policy = "AsAgent")]
    [Route("agency/query")]
    public class AgencyQueryController : Controller
    {
        private readonly IAgencySubjectQueryService agencySubjectQueryService;
        private readonly ISampleDataTypeMapper sampleDataTypeMapper;

        public AgencyQueryController(IAgencySubjectQueryService agencySubjectQueryService, ISampleDataTypeMapper sampleDataTypeMapper)
        {
            this.agencySubjectQueryService = agencySubjectQueryService;
            this.sampleDataTypeMapper = sampleDataTypeMapper;
        }

        [Route("")]
        public async Task<IActionResult> Index([FromQuery] IEnumerable<string> type, [FromQuery] AgencySubjectQueryParameters parameters)
        {
            var agencySubject = await this.agencySubjectQueryService.GetAgencySubjectAsync(parameters.Subject);

            if (!type.Any())
            {
                type = this.sampleDataTypeMapper.GetAvailableSampleDataTypes(agencySubject.HealthDataScopes.Select(s => s.Name)).Select(s => s.Slug);
            }

            if (agencySubject == null)
            {
                return RedirectToAction("Index", "AgencyUserList");
            }

            var sampleTypes = type
                .Select(t => this.sampleDataTypeMapper.ConvertToSampleDataType(t))
                .Where(t => t != null)
                .Select(t => t.Value);
            
            var model = new AgencyQueryViewModel(agencySubject)
            {
                Types = sampleTypes
            };

            return this.View(model);
        }

        [Route("{type:sampletype}")]
        public async Task<IActionResult> Index(string type, [FromQuery] AgencySubjectQueryParameters parameters)
        {
            return await Index(new[] {type }, parameters);
        }
    }
}
