using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CoreHealthCheck.HealthCheckApiiDemo.CustomHealthChecksProvider.DatabaseHealthChecks.SqlServerHealthChecks;
using CoreHealthCheck.HealthCheckApiiDemo.CustomHealthChecksProvider.PingHealthChecks;
using CoreHealthCheck.HealthCheckApiiDemo.Middlewares;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoreHealthCheck.HealthCheckApiiDemo
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }




        public void ConfigureServices(IServiceCollection services)
        { 
            services.AddCustomHealthChecksMiddleware(Configuration);
            services.AddHealthChecksUI();


            services.AddControllers();
        }








        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region ....
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting(); 
            app.UseAuthorization();
            #endregion

            #region HealthCkecks ayarları
            app.UseHealthChecks(
                   path: "/AdemOlguner-HealthChecks",
                   options: new HealthCheckOptions()
                   {
                       Predicate = _ => true,
                       ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                   });

            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/AdemOlguner-HealthChecks-ui";
                options.AddCustomStylesheet("Content/css/ademolguner-healthchecks-custom.css");
            });

            app.UseHealthChecks(
                   path: "/AdemOlguner-HealthChecks-databases",
                   options: new HealthCheckOptions()
                   {
                       Predicate = _ => true,
                       ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                   });
            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/AdemOlguner-HealthChecks-ui";
            });
            #endregion

            #region ....

            app.UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               });
            #endregion
        }


         
    }
}
