using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Scraper.Database;
using Scraper.Scheduler;
using StructureMap;

namespace Scraper.Structuremap
{
    public class ScraperRegistry : Registry
    {
        public ScraperRegistry()
        {
            For<IDataStore>().Use<DataStore>().Singleton();
            For<IScraperScheduler>().Use<ScraperScheduler>().Singleton();
            For<IJobFactory>().Use<StructureMapJobFactory>().Singleton();
        }
    }
}
