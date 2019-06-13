using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Topshelf;

namespace Demoservice
{
    public class MyServiceControl : ServiceControl
    {
        private readonly MyService _clientService;
        private readonly ILogger<MyServiceControl> _logger;
        private CancellationTokenSource _cts;
        private Task _executingTask;

        public MyServiceControl(
            MyService clientService,
            ILogger<MyServiceControl> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        public bool Start(HostControl hostControl)
        {
            _logger.LogInformation("Service Started.");

            _cts = new CancellationTokenSource();

            // Store the task we're executing
            _executingTask = ExecuteAsync(_cts.Token);

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            if (_executingTask == null)
            {
                return true;
            }

            // Signal cancellation to the executing method
            _cts.Cancel();

            // Wait until the task completes or the stop token triggers
            Task.WhenAny(_executingTask, Task.Delay(1500))
                .GetAwaiter()
                .GetResult();

            return true;
        }

        private async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _clientService.StartAsync(cancellationToken)
                                .ConfigureAwait(false);
        }
    }
}