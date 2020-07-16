using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MuKai_Auth
{
    public class ConsulHostService : IHostedService
    {
        private readonly IConsulClient consulClient;
        private readonly IConfiguration configuration;
        private CancellationTokenSource cts;
        private string serviceId;
        public ConsulHostService(IConsulClient consulClient, IConfiguration configuration)
        {
            this.consulClient = consulClient;
            this.configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var name = Dns.GetHostName(); // get container id
            var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            string address = ip.ToString();
            int port = int.Parse(configuration["Port"]);
            serviceId = "service:" + Guid.NewGuid();
            var registration = new AgentServiceRegistration()
            {
                ID = serviceId,
                Name = "Auth Service",
                Address = address,
                Port = port,
                Tags = new[] { "auth" },
                Check = new AgentServiceCheck()
                {
                    Interval = TimeSpan.FromSeconds(20),
                    HTTP = $"http://{address}:{port}/health",
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
