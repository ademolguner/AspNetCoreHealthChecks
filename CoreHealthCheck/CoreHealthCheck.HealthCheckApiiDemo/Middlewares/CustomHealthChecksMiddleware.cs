using CoreHealthCheck.HealthCheckApiiDemo.CustomHealthChecksProvider.DatabaseHealthChecks.SqlServerHealthChecks;
using CoreHealthCheck.HealthCheckApiiDemo.CustomHealthChecksProvider.PingHealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace CoreHealthCheck.HealthCheckApiiDemo.Middlewares
{
    public static class CustomHealthChecksMiddleware
    { 
        public static IHealthChecksBuilder AddCustomHealthChecksMiddleware(this IServiceCollection services, IConfiguration configuration)
        {
            var builder = services.AddHealthChecks();
            builder.AddCheck<BenimKuryemHealthChecksProvider>("BenimKuryem DataBase HealthCheck");
            builder.AddCheck<EgitimPortalHealthChecksProvider>("EgitimPortal DataBase HealthCheck");
            builder.AddSqlServer(
                                 connectionString: configuration.GetSection("ConnectionStrings:SqlConnections:EgitimPortal").Value,
                                 healthQuery: "select 1",
                                 name: "EgitimPortal AddSqlServer Sql Server Db Check",
                                 failureStatus: null,
                                 tags: null,
                                 timeout: TimeSpan.FromMinutes(1));
            builder.AddSqlServer(
                                connectionString: configuration.GetSection("ConnectionStrings:SqlConnections:BenimKuryem").Value,
                                healthQuery: "select 1",
                                name: "BenimKuryem AddSqlServer Sql Server Db Check",
                                failureStatus: null,
                                tags: null,
                                timeout: TimeSpan.FromMinutes(1));
            builder.AddPingHealthCheck(_ =>
            {
                _.AddHost("google.com", 200); // _.AddHost("172.217.17.196", 200);
            }, "Google ping Test");
            builder.AddCheck("Google ping Test 2", new PingHealthCheckProvider("google.com", 10));
            builder.AddCheck("Google ping Test 3", new PingHealthCheckProvider("google.com", 200));
            builder.AddCheck("Google ping Test 4", new PingHealthCheckProvider("googleamca.com", 200));
            builder.AddSmtpHealthCheck(_ =>
            {
                _.Host = "smtp.gmail.com";
                _.Port = 25;
                _.ConnectionType = HealthChecks.Network.Core.SmtpConnectionType.TLS;
            }, "Gmail SMTP Ping");
            builder.AddDiskStorageHealthCheck(s => s.AddDrive("C:\\", 1024)); // 1024 MB (1 GB) free minimum
            builder.AddVirtualMemorySizeHealthCheck(512); // 512 MB max allocated memory
            builder.AddPrivateMemoryHealthCheck(512); // 512 MB max allocated memory
            builder.AddProcessHealthCheck("chrome.exe", p => p.Length > 0); // check if process is running
            builder.AddWindowsServiceHealthCheck("docker", s => s.Status == ServiceControllerStatus.Running, "docker windows service status");
            return builder;
        }

    }
}
