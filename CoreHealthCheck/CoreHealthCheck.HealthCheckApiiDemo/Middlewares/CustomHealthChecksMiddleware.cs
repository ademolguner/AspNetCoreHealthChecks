using CoreHealthCheck.HealthCheckApiiDemo.CustomHealthChecksProvider.DatabaseHealthChecks.SqlServerHealthChecks;
using CoreHealthCheck.HealthCheckApiiDemo.CustomHealthChecksProvider.PingHealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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

            //  AddHealthChecks()
            builder.AddSqlServer(configuration.GetSection("ConnectionStrings:SqlConnections:BenimKuryem").Value, "select 1", "BenimKuryem addsql",null,null,TimeSpan.FromMinutes(5));
            builder.AddSqlServer(configuration.GetSection("ConnectionStrings:SqlConnections:EgitimPortal").Value, "select 2", "EgitimPortal addsql", null, null, TimeSpan.FromMinutes(5));


            builder.AddPingHealthCheck(_ =>
            {
                _.AddHost("google.com", 200); // _.AddHost("172.217.17.196", 200);
            }, "Google ping Test");
            builder.AddCheck("Google ping Test 2", new PingHealthCheckProvider("google.com", 200));

            builder.AddSmtpHealthCheck(_ =>
            {
                _.Host = "smtp.gmail.com";
                _.Port = 25;
                _.ConnectionType = HealthChecks.Network.Core.SmtpConnectionType.TLS;
            }, "Gmail SMTP Ping");



            //services
            //   .AddHealthChecksUI()
            //   .AddHealthChecks()
            //   .AddCheck<BenimKuryemHealthChecksProvider>("BenimKuryem DataBase HealthCheck")
            //   .AddCheck<EgitimPortalHealthChecksProvider>("EgitimPortal DataBase HealthCheck")
            //   .AddPingHealthCheck(_ =>
            //   {
            //       _.AddHost("172.217.17.196", 200);
            //   }, "Google ping Test")
            //   .AddSmtpHealthCheck(_ =>
            //   {
            //       _.Host = "smtp.gmail.com";
            //       _.Port = 25;
            //       _.ConnectionType = HealthChecks.Network.Core.SmtpConnectionType.TLS;
            //   }, "Gmail MSTP");


            return builder;

        }

    }
}
