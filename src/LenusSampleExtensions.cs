using System;
using System.Threading.Tasks;
using Clinician.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using Clinician.Services.Impl;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using Clinician.ApiClients.HealthClient;
using System.Security.Claims;
using Clinician.ApiClients.AgencyClient;
using Refit;
using Polly;
using System.Linq;

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
                .AddCookie()
                .AddOpenIdConnect(o =>
                {
                    configuration.GetSection("OpenIdConnect").Bind(o);

                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                    JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add(JwtClaimTypes.Subject, ClaimTypes.NameIdentifier);
                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add(JwtClaimTypes.Role, JwtClaimTypes.Role);

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
                    o.ResponseType = OpenIdConnectResponseType.CodeIdToken;

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
                        var role = context.User.RootElement.GetProperty(JwtClaimTypes.Role);
                        var roleNames = role.ValueKind == System.Text.Json.JsonValueKind.Array ? role.EnumerateArray().Select(e => e.GetString()) : new[] { role.GetString() };

                        var claims = roleNames.Select(rn => new Claim(JwtClaimTypes.Role, rn)).ToList();
                        var id = context.Principal.Identity as ClaimsIdentity;
                        id?.AddClaims(claims);

                        return Task.CompletedTask;
                    };
                });

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

            serviceCollection.AddTransient(typeof(AgencyApiClientHttpClientHandler));

            serviceCollection.AddRefitClient<IAgencyApiClient>()
                .ConfigureHttpClient(c =>
                {
                    var options = configuration.GetSection("Agency").Get<HealthPlatformAgencyOptions>();

                    c.BaseAddress = options.AgencyApiBaseUri;
                })
                .AddHttpMessageHandler<AgencyApiClientHttpClientHandler>()
                .AddTransientHttpErrorPolicy(
                    builder => builder.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    )
                );

            return serviceCollection;
        }

        public static IServiceCollection AddLenusHealthClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            serviceCollection.AddSingleton<IAccessTokenAccessor, AccessTokenAccessor>();
            serviceCollection.AddOptions();

            serviceCollection.AddTransient(typeof(HealthDataClientHttpClientHandler));

            serviceCollection.AddRefitClient<IHealthDataClient>()
                .ConfigureHttpClient(c =>
                {
                    var options = configuration.GetSection("HealthDataClient").Get<HealthDataClientOptions>();

                    c.BaseAddress = options.BaseUri;
                })
                .AddHttpMessageHandler<HealthDataClientHttpClientHandler>()
                .AddTransientHttpErrorPolicy(
                    builder => builder.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    )
                );

            return serviceCollection;
        }
    }
}
