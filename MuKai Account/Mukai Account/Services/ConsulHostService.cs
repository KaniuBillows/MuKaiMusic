using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Mukai_Account.Service
{
    public class ConsulHostService : IHostedService
    {
        private readonly IConfiguration configuration;
        private readonly IConsulClient client;
        private CancellationTokenSource cts;
        private string serviceId;
        public ConsulHostService(IConfiguration configuration,
                                IConsulClient client)
        {
            this.configuration = configuration;
            this.client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var name = Dns.GetHostName(); // get container id
            var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            string address = ip.ToString();
            serviceId = "service:" + Guid.NewGuid();
            var registration = new AgentServiceRegistration()
            {
                ID = serviceId,
                Name = "Mukai Account",
                Address = address,
                Port = int.Parse(configuration["Port"]),
                Tags = new[] { "api" },
                Check = new AgentServiceCheck()
                {
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = $"http://{address}:{configuration["Port"]}/api/health/index",
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60)
                }
            };
            // 首先移除服务，避免重复注册
            await client.Agent.ServiceDeregister(registration.ID, cts.Token);
            await client.Agent.ServiceRegister(registration, cts.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            cts.Cancel();
            await client.Agent.ServiceDeregister(serviceId, cancellationToken);
        }
    }
}