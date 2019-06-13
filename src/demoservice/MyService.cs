using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Demoservice
{
    public class MyService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly ILogger<MyService> _logger;

        public MyService(ILogger<MyService> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Hello World!");
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}