using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Demoservice.Jobs
{
    public class HelloJob : IJob, IDisposable
    {
        private readonly ILogger<HelloJob> _logger;

        public HelloJob(ILogger<HelloJob> logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var cancellationToken = context.CancellationToken;

            await Task.Delay(1000, cancellationToken);

            var lastRun = context.PreviousFireTimeUtc?.DateTime.ToString() ?? string.Empty;
            _logger.LogInformation("Greetings from HelloJob! Previous run: {lastRun}", lastRun);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // todo resources disposen
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}