using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace CoreHealthCheck.HealthCheckApiiDemo.CustomHealthChecksProvider.PingHealthChecks
{
    public class PingHealthCheckProvider : IHealthCheck
    {
        private readonly string _host;
        private readonly int _timeout;
        public PingHealthCheckProvider(string host, int timeout) { _host = host; _timeout = timeout; }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(_host, _timeout);
                if (reply.Status != IPStatus.Success)
                {
                    return HealthCheckResult.Unhealthy(_host + " adresine erişim yapılamamktadır.");
                }
                if (reply.RoundtripTime >= _timeout)
                {
                    return HealthCheckResult.Degraded(_host + " adresine atılan ping istenilen zamanda erişilemedi."
                                                            + " Erişim süresi :" + reply.RoundtripTime.ToString());
                }
                return HealthCheckResult.Healthy();
            }
            catch(NetworkInformationException ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message.ToString());
            }
        }
    }
}
