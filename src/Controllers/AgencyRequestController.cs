using System;
using Clinician.ApiClients.AgencyClient;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Clinician.Controllers
{
    [Authorize(Policy = "AsAgent")]
    [Route("agency/request")]
    public class AgencyRequestController : Controller
    {
        private readonly IOptions<HealthPlatformAgencyOptions> agencyOptions;
        private readonly IOptions<OpenIdConnectOptions> oidcOptions;

        public AgencyRequestController(IOptions<HealthPlatformAgencyOptions> agencyOptions, IOptions<OpenIdConnectOptions> oidcOptions)
        {
            this.agencyOptions = agencyOptions;
            this.oidcOptions = oidcOptions;
        }

        [Route("")]
        public IActionResult Index()
        {
            var queryString = QueryString
                .Create(nameof(this.oidcOptions.Value.ClientId).ToLowerInvariant(),
                    this.oidcOptions.Value?.ClientId ?? string.Empty);
            
            var agencyRequestUriBuilder = new UriBuilder(this.agencyOptions.Value.AgencyRequestUri)
            {
                Query = queryString.ToString()
            };
            return Redirect(agencyRequestUriBuilder.Uri.ToString());
        }
    }
}
