using System.Collections.Generic;
using System.Threading.Tasks;
using Clinician.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Clinician.Services.Impl;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.Extensions.Options;
using Clinician.ApiClients.HealthClient;
using System.Net.Http;
using System.Security.Claims;
using Clinician.ApiClients.AgencyClient;
using Refit;
using Clinician.Controllers;
using Clinician.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Clinician
{
    public static class LenusSampleExtensions 
    {
        public static IServiceCollection AddLenusAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<OpenIdConnectOptions>(configuration.GetSection("OpenIdConnect"));

            serviceCollection
                .AddAuthentication(o =>
                {
                    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(o =>
                {
                    //o.AccessDeniedPath = "/error";
                    //o.LoginPath = "/login";
                    //o.LogoutPath = "/logout";
                })
                .AddOpenIdConnect(o =>
                {
                    configuration.GetSection("OpenIdConnect").Bind(o);

                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                    JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add(JwtClaimTypes.Subject, ClaimTypes.NameIdentifier);

                    o.TokenValidationParameters = new TokenValidationParameters()
                    {
                        RoleClaimType = JwtClaimTypes.Role,
                        NameClaimType = JwtClaimTypes.Name
                    };

                    configuration.GetSection("OpenIdConnect").Bind(o);
                    /* save the access token within an authentication cookie */
                    o.SaveTokens = true;
                    /* match token and cookie lifetime */
                    o.UseTokenLifetime = true;
                    
                    o.GetClaimsFromUserInfoEndpoint = true;
                    
                    /* use the authorization_code flow */
                    o.ResponseType = OpenIdConnectResponseType.Code;

                    o.Events.OnRemoteFailure += ctx =>
                    {
                        if (ctx.Failure != null)
                        {
                            ctx.Response.Redirect($"/error?msg={ctx.Failure.Message}");
                        }
                        else
                        {
                            ctx.Response.Redirect("/");
                        }

                        ctx.HandleResponse();
                        return Task.CompletedTask;
                    };

                    /* Mandatory scope */
                    o.Scope.Add("openid");

                    /* I want profile information (givenname, familyname) */
                    o.Scope.Add("profile");

                    /* I want to read email address */
                    o.Scope.Add("email");

                    /* I want to act as an agent */
                    o.Scope.Add("agency_api");

                    // Ensures the "role" claim is mapped into the ClaimsIdentity
                    o.Events.OnUserInformationReceived = context =>
                    {
                        if (context.User.TryGetValue(JwtClaimTypes.Role, out var role))
                        {
                            var roleNames = role.Type == JTokenType.Array 
                                ? role.Select(x => (string)x) 
                                : new[] { (string)role };
                            var claims = roleNames.Select(rn => new Claim(JwtClaimTypes.Role, rn));
                            var id = context.Principal.Identity as ClaimsIdentity;
                            id?.AddClaims(claims);
                        }
                        return Task.CompletedTask;
                    };
                })
                ;

            return serviceCollection;
        }

        public static IServiceCollection AddLenusAuthorisation(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddAuthorization(o =>
            {
                o.AddPolicy("AsAgent", policy => policy.RequireAuthenticatedUser().RequireRole("agent"));
            });
        }

        public static IServiceCollection AddAgencyServices(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.AddOptions();
            serviceCollection.Configure<HealthPlatformAgencyOptions>(configuration.GetSection("Agency"));
            
            serviceCollection.AddSingleton<ISampleMapper, SampleMapper>();
            
            serviceCollection.AddSingleton<ISampleDataTypeMapper, SampleDataTypeMapper>();

            serviceCollection.AddScoped<IAgencySubjectQueryService, AgencySubjectQueryService>();

            serviceCollection.AddScoped(s =>
            {
                var options = s.GetRequiredService<IOptions<HealthPlatformAgencyOptions>>().Value;
                var client = new HttpClient(new AgencyApiClientHttpClientHandler(s.GetRequiredService<IAccessTokenAccessor>()))
                {
                    BaseAddress = options.AgencyApiBaseUri
                };
                return RestService.For<IAgencyApiClient>(client);
            });

            return serviceCollection;
        }

        public static IServiceCollection AddLenusHealthClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            serviceCollection.AddSingleton<IAccessTokenAccessor, AccessTokenAccessor>();
            serviceCollection.AddOptions();

            serviceCollection.Configure<HealthDataClientOptions>(configuration.GetSection("HealthDataClient"));

            serviceCollection.AddScoped(s =>
            {
                var options = s.GetRequiredService<IOptions<HealthDataClientOptions>>().Value;
                var client = new HttpClient(new HealthDataClientHttpClientHandler(s.GetRequiredService<IAccessTokenAccessor>(), s.GetService<ILogger<HealthDataClientHttpClientHandler>>()))
                {
                    BaseAddress = options.BaseUri
                };
                return RestService.For<IHealthDataClient>(client);
            });
            return serviceCollection;
        }
    }
}
