using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Microsoft.AspNetCore.Routing;
using Clinician.Controllers;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Clinician
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization();
            services.AddMvc().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
            
            services.Configure<RouteOptions>(r =>
            {
                r.LowercaseUrls = true;
                r.ConstraintMap.Add("sampletype", typeof(SampleDataTypeRouteConstraint));
            });

            services.AddLenusAuthentication(Configuration);
            services.AddLenusAuthorisation();
            services.AddLenusHealthClient(this.Configuration);
            services.AddAgencyServices(this.Configuration);

            services.AddMemoryCache();
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();
            app.UseRequestLocalization();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseStaticFiles();

            var gbCulture = CultureInfo.GetCultureInfo("en-GB");
            app.UseRequestLocalization(
                new RequestLocalizationOptions()
                {
                    DefaultRequestCulture = new RequestCulture(gbCulture),
                    SupportedCultures = new[] { gbCulture },
                    SupportedUICultures = new[] { gbCulture }
                });

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(e =>
            {
                e.MapDefaultControllerRoute();
            });
        }
    }
}
