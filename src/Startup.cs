using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Clinician.Controllers;

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
            services.AddMvc();
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

            services.Configure<MvcJsonOptions>(o =>
            {
                o.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                o.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                o.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseMvc();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
