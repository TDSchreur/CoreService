using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Demoservice.Jobs;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Demoservice
{
    public class MyService : BackgroundService
    {
        private readonly StdSchedulerFactory _factory;
        private readonly IJobFactory _jobFactory;

        public MyService(IJobFactory jobFactory)
        {
            _jobFactory = jobFactory;

            var props = new NameValueCollection
            {
                {"quartz.serializer.type", "binary"},
                {"quartz.scheduler.instanceName", "MyScheduler"},
                {"quartz.jobStore.type", "Quartz.Simpl.RAMJobStore, Quartz"},
                {"quartz.threadPool.threadCount", "3"}
            };
            _factory = new StdSchedulerFactory(props);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scheduler = await _factory.GetScheduler(stoppingToken);
            scheduler.JobFactory = _jobFactory;

            var helloJob = JobBuilder.CreateForAsync<HelloJob>()
                                     .WithIdentity(nameof(HelloJob))
                                     .Build();

            var trigger = TriggerBuilder.Create()
                                        .WithIdentity("EveryMinuteTrigger")
                                        //.StartNow()
                                        .WithCronSchedule("0/5 * * * * ?")
                                        //.WithSimpleSchedule(builder => builder.WithIntervalInSeconds(5)
                                        //                                      .RepeatForever())
                                        .Build();

            await scheduler.ScheduleJob(helloJob, trigger, stoppingToken);

            await scheduler.Start(stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}