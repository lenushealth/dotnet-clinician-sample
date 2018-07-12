using System;
using System.Collections.Generic;
using System.Linq;
using Clinician.ApiClients.AgencyClient;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
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
            var kvps = new[]
            {
                (nameof(oidcOptions.Value.ClientId).ToLowerInvariant(), oidcOptions.Value?.ClientId ?? string.Empty),
                ("redirectUrl", Url.Action("Index", "Home", null, Request.GetUri().Scheme))
            }.Select(tpl => new KeyValuePair<string, string>(tpl.Item1, tpl.Item2));

            var url = new UriBuilder(agencyOptions.Value.AgencyRequestUri)
            {
                Query = QueryString.Create(kvps).ToString()
            }.Uri.ToString();

            return Redirect(url);
        }
    }
}