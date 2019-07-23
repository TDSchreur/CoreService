using System;
using Autofac;
using Quartz;
using Quartz.Spi;

namespace Demoservice
{
    public class MyJobFactory : IJobFactory
    {
        private readonly IComponentContext _container;

        public MyJobFactory(IComponentContext container)
        {
            _container = container;
        }

        
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _container.Resolve(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}