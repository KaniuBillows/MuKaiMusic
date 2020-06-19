using System;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Netease_API
{
    public class ConsulHostService : IHostedService
    {
        private readonly IConsulClient consulClient;
        private readonly IConfiguration configuration;
        private CancellationTokenSource cts;
        private string serviceId;
        public ConsulHostService(IConsulClient consulClient,IConfiguration configuration)
        {
            this.consulClient = consulClient;
            this.configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var uri = new Uri(configuration["Address"]);
            serviceId = "service:" + Guid.NewGuid();
            var registration = new AgentServiceRegistration()
            {
                ID = serviceId,
                Name = "Netease API",
                Address = uri.Host,
                Port = uri.Port,
                Tags = new[] { "api" },
                Check = new AgentServiceCheck()
                {
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = $"http://{uri.Host}:{uri.Port}/api/health/index",
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60)
                }
            };
            // 首先移除服务，避免重复注册
            await consulClient.Agent.ServiceDeregister(registration.ID, cts.Token);
            await consulClient.Agent.ServiceRegister(registration, cts.Token);
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            cts.Cancel();
            await consulClient.Agent.ServiceDeregister(serviceId, cancellationToken);
        }
    }
}
