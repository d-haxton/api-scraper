using Quartz.Spi;
using Scraper.Database;
using Scraper.Scheduler;
using Scraper.Scraper;
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
            For<IHttpClientWrapper>().Use<HttpClientWrapper>().Singleton();
        }
    }
}
