using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Kuwo_API
{
    public class ConsulHostService : IHostedService
    {
        private readonly IConsulClient _client;
        private readonly IConfiguration _configuration;
        private CancellationTokenSource _cts;
        private string _serviceId;

        public ConsulHostService(IConsulClient consulClient, IConfiguration configuration)
        {
            this._client = consulClient;
            this._configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var name = Dns.GetHostName(); // get container id
            IPAddress ip =
                (await Dns.GetHostEntryAsync(name)).AddressList.FirstOrDefault(x =>
                    x.AddressFamily == AddressFamily.InterNetwork);
            var address = ip.ToString();
            var port = int.Parse(_configuration["Port"]);
            _serviceId = "service:" + Guid.NewGuid();
            AgentServiceRegistration registration = new AgentServiceRegistration()
            {
                ID = _serviceId,
                Name = "Kuwo API",
                Address = address,
                Port = port,
                Tags = new[] {"api"},
                Check = new AgentServiceCheck()
                {
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = $"http://{address}:{port}/api/health/index",
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60)
                }
            };
            // 首先移除服务，避免重复注册
            await _client.Agent.ServiceDeregister(registration.ID, _cts.Token);
            await _client.Agent.ServiceRegister(registration, _cts.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            await _client.Agent.ServiceDeregister(_serviceId, cancellationToken);
        }
    }
}