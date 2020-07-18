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
//using CoreHealthCheck.HealthCheckApiiDemo.CustomHealthChecksProvider.DatabaseHealthChecks.SqlServerHealthChecks;
//using CoreHealthCheck.HealthCheckApiiDemo.Middlewares;
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
            #region .....
            //services.AddCustomHealthChecksMiddleware(Configuration);
            //services.AddHealthChecksUI();
            #endregion
            services
                .AddHealthChecksUI(
                setupSettings: 
                setup =>
                {
                    setup.AddHealthCheckEndpoint("Test Health Check", "http://localhost:32986/AdemOlguner-HealthChecks");
                    setup.SetEvaluationTimeInSeconds(3);
                    setup.SetEvaluationTimeInSeconds(60);
                })
                .AddHealthChecks()
                 .AddSqlServer(
                                 connectionString: Configuration.GetSection("ConnectionStrings:SqlConnections:EgitimPortal").Value,
                                 healthQuery: "select 1",
                                 name: "EgitimPortal AddSqlServer Sql Server Db Check",
                                 failureStatus: null,
                                 tags: null,
                                 timeout: TimeSpan.FromMilliseconds(1))
                 .AddCheck<EgitimPortalHealthChecksProvider>("EgitimPortal EgitimPortalHealthChecksProvider Check")
                .AddPingHealthCheck(_ =>
                {
                    _.AddHost("google.com", 200); // _.AddHost("172.217.17.196", 200);
                }, "Google ping Test")
                .AddCheck("Ping Test 1",new PingHealthCheckProvider("google.com",250))
                .AddCheck("Ping Test 2", new PingHealthCheckProvider("google.com", 3))
                .AddCheck("Ping Test 3", new PingHealthCheckProvider("1.9.0.3", 250))
                .AddDiskStorageHealthCheck(s => s.AddDrive("C:\\", 1024)) // 1024 MB (1 GB) free minimum
                .AddVirtualMemorySizeHealthCheck(512) // 512 MB max allocated memory
                .AddPrivateMemoryHealthCheck(512) // 512 MB max allocated memory
                .AddProcessHealthCheck("chrome.exe", p => p.Length > 0) // check if process is running
                .AddWindowsServiceHealthCheck("docker", s => s.Status == ServiceControllerStatus.Running, "docker windows service status")
                 ;

             

            #region ....
            services.AddControllers();
            #endregion
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




        public class RandomHealthCheck : IHealthCheck
        {
            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                if (DateTime.UtcNow.Minute % 2 == 0)
                {
                    return Task.FromResult(HealthCheckResult.Healthy());
                }

                return Task.FromResult(HealthCheckResult.Unhealthy(description: "failed", exception: new InvalidCastException("Invalid cast from to to to")));
            }

        }
    }
}
