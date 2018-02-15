using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Scraper.Model;

namespace Scraper.Database
{
    public interface IDataStore
    {
        ScrapeRequest FindScrapeRequestByGuid(string guid);
    }

    public class DataStore : IDataStore
    {
        private readonly IDatabaseConfiguration configuration;

        public DataStore(IDatabaseConfiguration configuration)
        {
            this.configuration = configuration;

            EnsureIndicies();
        }

        private void EnsureIndicies()
        {
            using (var db = new LiteDatabase(configuration.DatabaseFileName))
            {
                var scrapes = db.GetCollection<ScrapeRequest>(configuration.ScrapeRequestTable);
                scrapes.EnsureIndex(x => x.Guid, unique: true);
            }
        }

        public void AddNewScrapeRequest(IEnumerable<ScrapeRequest> request)
        {
            using (var db = new LiteDatabase(configuration.DatabaseFileName))
            {
                var scrapes = db.GetCollection<ScrapeRequest>(configuration.ScrapeRequestTable);
                scrapes.InsertBulk(request);
            }
        }

        public void AddNewScrapeRequest(ScrapeRequest request)
        {
            using (var db = new LiteDatabase(configuration.DatabaseFileName))
            {
                var scrapes = db.GetCollection<ScrapeRequest>(configuration.ScrapeRequestTable);
                scrapes.Insert(request);
            }
        }

        public ScrapeRequest FindScrapeRequestByGuid(string guid)
        {
            using (var db = new LiteDatabase(configuration.DatabaseFileName))
            {
                var scrapes = db.GetCollection<ScrapeRequest>(configuration.ScrapeRequestTable);

                return scrapes.FindOne(x => x.Guid == guid);
            }
        }
    }
}
