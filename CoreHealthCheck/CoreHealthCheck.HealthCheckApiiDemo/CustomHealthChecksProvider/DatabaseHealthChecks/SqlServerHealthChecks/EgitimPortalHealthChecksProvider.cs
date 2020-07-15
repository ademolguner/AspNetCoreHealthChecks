using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreHealthCheck.HealthCheckApiiDemo.CustomHealthChecksProvider.DatabaseHealthChecks.SqlServerHealthChecks
{
    public class EgitimPortalHealthChecksProvider : IHealthCheck
    {
        public EgitimPortalHealthChecksProvider(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {

            using (var conn = new SqlConnection(Configuration.GetSection("ConnectionStrings:SqlConnections:EgitimPortal").Value))
            {
                try
                {
                    conn.Open();
                    return await Task.FromResult(HealthCheckResult.Healthy());
                }
                catch (SqlException ex)
                {

                    return await Task.FromResult(HealthCheckResult.Unhealthy());
                }
            }
        }
    }
}
