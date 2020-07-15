using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CoreHealthCheck.HealthCheckApiiDemo.CustomHealthChecksProvider.DatabaseHealthChecks.SqlServerHealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoreHealthCheck.HealthCheckApiiDemo
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
            services
                .AddHealthChecksUI()
                .AddHealthChecks()
                .AddCheck<BenimKuryemHealthChecksProvider>("BenimKuryem DataBase HealthCheck")
                .AddCheck<EgitimPortalHealthChecksProvider>("EgitimPortal DataBase HealthCheck")
                .AddPingHealthCheck(_ =>
                {
                    _.AddHost("172.217.17.196", 200);
                }, "Google ping Test")
                .AddSmtpHealthCheck(_ =>
                {
                    _.Host = "smtp.gmail.com";
                    _.Port = 25;
                    _.ConnectionType = HealthChecks.Network.Core.SmtpConnectionType.TLS;
                }, "Gmail MSTP");



            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseHealthChecks("/AdemOlguner-HealthChecks", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                //ResponseWriter = async (a, o) =>
                //{
                //    a.Response.ContentType = "application/json";
                //    var results = JsonConvert.SerializeObject(new
                //    {
                //        status = o.Status.ToString(),
                //        errors = o.Entries.Select(ao => new { Key = ao.Key, value = ao.Value.Status.ToString() }),
                //        durations=o.TotalDuration.ToString()
                //    });
                //    await a.Response.WriteAsync(results);
                //}

            });

            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/AdemOlguner-HealthChecks-ui";
            });



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
