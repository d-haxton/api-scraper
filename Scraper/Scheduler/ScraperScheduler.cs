using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace Scraper.Scheduler
{
    public class ScraperScheduler
    {
        private readonly ISchedulerFactory factory;
        private readonly IScheduler scheduler;

        public ScraperScheduler()
        {
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };

            factory = new StdSchedulerFactory(props);
            scheduler = factory.GetScheduler().GetAwaiter().GetResult();
        }

        public async Task Start(CancellationToken cancellationToken = default(CancellationToken))
        {
            await scheduler.Start(cancellationToken);
        }

        public async Task Stop(bool finishJobs = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            await scheduler.Shutdown(finishJobs, cancellationToken);
        }
    }
}
