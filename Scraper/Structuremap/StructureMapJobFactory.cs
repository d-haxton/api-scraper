using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Spi;
using StructureMap;

namespace Scraper.Structuremap
{
    public class StructureMapJobFactory : IJobFactory
    {
        private readonly IContainer container;

        public StructureMapJobFactory(IContainer container)
        {
            this.container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob) container.GetInstance(bundle.JobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (job is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
