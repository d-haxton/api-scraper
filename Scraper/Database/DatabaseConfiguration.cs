using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.Database
{
    public interface IDatabaseConfiguration
    {
        string DatabaseFileName { get; }
        string ScrapeRequestTable { get; }
        string ScrapeResultTable { get; }
    }

    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        public string DatabaseFileName { get; } = "ScraperDb.db";
        public string ScrapeRequestTable { get; } = "scraperequests";
        public string ScrapeResultTable { get; } = "scraperesults";
    }
}
