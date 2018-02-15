using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scraper.Database;
using StructureMap;

namespace Scraper.Structuremap
{
    public class ScraperRegistry : Registry
    {
        public ScraperRegistry()
        {
            For<IDataStore>().Use<DataStore>().Singleton();
        }
    }
}
