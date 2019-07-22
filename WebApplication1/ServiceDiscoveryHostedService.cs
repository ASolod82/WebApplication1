using Consul;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class ServiceDiscoveryHostedService : IHostedService
    {
        private readonly IConsulClient _client;
        private readonly ServiceConfig _config;
        private string _registrationId;

        public ServiceDiscoveryHostedService(IConsulClient client, ServiceConfig config)
        {
            _client = client;
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _registrationId = $"{_config.ServiceName}-{_config.ServiceId}";

            var healhurl = _config.ServiceAddress + "api/health/status";
            Console.WriteLine($"Healhcheck url : {healhurl}");

            var registration = new AgentServiceRegistration
            {
                ID = _registrationId,
                Name = _config.ServiceName,
                Address = _config.ServiceAddress.Host,
                Port = _config.ServiceAddress.Port,
                Check = new AgentCheckRegistration()
                {
                    HTTP = healhurl,
                    Notes = "Checks api/health/status on Bridge localhost",
                    Timeout = TimeSpan.FromSeconds(3),
                    Interval = TimeSpan.FromSeconds(10)
                }
            };

            await _client.Agent.ServiceDeregister(registration.ID, cancellationToken);
            await _client.Agent.ServiceRegister(registration, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.Agent.ServiceDeregister(_registrationId, cancellationToken);
        }

    }
}
