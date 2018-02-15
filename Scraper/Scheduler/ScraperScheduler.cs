using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Scraper.Scheduler
{
    public interface IScraperScheduler
    {
        /// <summary>
        /// Async Task that will Build and Schedule a number of worker threads that will consume the latest Accepted jobs in ScrapeRequest
        /// </summary>
        /// <param name="jobCount">The amount of worker threads you want to spawn</param>
        Task BuildAndScheduleJob(int jobCount);

        /// <summary>
        /// Starts the thread pool scheduler
        /// </summary>
        Task Start(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Stops the thread pool scheduler
        /// </summary>
        /// <param name="finishJobs">if true will wait until all jobs are executed before shutting down</param>
        Task Stop(bool finishJobs = true, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class ScraperScheduler : IScraperScheduler
    {
        private readonly ISchedulerFactory factory;
        private readonly IScheduler scheduler;

        public ScraperScheduler(IJobFactory jobFactory)
        {
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };

            factory = new StdSchedulerFactory(props);
            scheduler = factory.GetScheduler().GetAwaiter().GetResult();
            scheduler.JobFactory = jobFactory;
        }

        public async Task BuildAndScheduleJob(int jobCount)
        {
            foreach (var i in Enumerable.Range(0, jobCount))
            {
                var jobDetail = BuildScraperJobDetail(i);
                var trigger = BuildScraperTrigger(i);
                await scheduler.ScheduleJob(jobDetail, trigger);
            }
        }

        private static IJobDetail BuildScraperJobDetail(int jobCount)
        {
            return JobBuilder.Create<ScraperJob>()
                .WithIdentity($"scraperJob{jobCount}", "scraperGroup")
                .Build();
        }

        private static ITrigger BuildScraperTrigger(int jobCount)
        {
            return TriggerBuilder.Create()
                .WithIdentity($"scraper{jobCount}Trigger", "scraperGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();
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
