using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace Scraper.Scheduler
{
    public class ScraperJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
        }
    }
}
