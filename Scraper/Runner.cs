using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Hosting.Self;
using Scraper.Scheduler;

namespace Scraper
{
    public interface IRunner
    {
        void Initialize();
    }

    public class Runner : IRunner
    {
        private readonly IScraperScheduler scraperScheduler;

        public Runner(IScraperScheduler scraperScheduler)
        {
            this.scraperScheduler = scraperScheduler;
        }

        public void Initialize()
        {
            scraperScheduler.Start();
            scraperScheduler.BuildAndScheduleJob(3);
        }
    }
}
