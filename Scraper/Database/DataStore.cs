using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using LiteDB;
using Scraper.Model;

namespace Scraper.Database
{
    public interface IDataStore
    {
        void AddNewScrapeRequest(IEnumerable<ScrapeRequest> request);
        void AddNewScrapeRequest(ScrapeRequest request);
        ScrapeRequest FindNextJob();
        ScrapeRequest FindScrapeRequestByGuid(string guid);
        ImmutableArray<ScrapeRequest> FindScrapeRequestsByUrl(string url);
        ImmutableArray<ScrapeResult> FindScrapeResultByUrl(string url);
        void InsertResult(ScrapeResult result);
        void UpdateRequest(ScrapeRequest request);
        ImmutableArray<ScrapeRequest> FindAllJobs();
        ImmutableArray<ScrapeRequest> FindRequests(Expression<Func<ScrapeRequest, bool>> predicate);
    }

    public class DataStore : IDataStore
    {
        private readonly object locker = new object();
        private readonly IDatabaseConfiguration configuration;
        private readonly LiteDatabase db;

        public DataStore(IDatabaseConfiguration configuration)
        {
            this.configuration = configuration;
            db = new LiteDatabase(configuration.DatabaseFileName);
        }


        public void AddNewScrapeRequest(IEnumerable<ScrapeRequest> request)
        {
            lock (locker)
            {
                var requests = db.GetCollection<ScrapeRequest>(configuration.ScrapeRequestTable).Include(x => x.Result);
                requests.InsertBulk(request);
            }
        }

        public void AddNewScrapeRequest(ScrapeRequest request)
        {
            lock (locker)
            {
                var requests = db.GetCollection<ScrapeRequest>(configuration.ScrapeRequestTable).Include(x => x.Result).Include(x => x.Result);
                requests.Insert(request);
            }
        }

        public ImmutableArray<ScrapeRequest> FindAllJobs()
        {
            lock (locker)
            {
                var requests = db.GetCollection<ScrapeRequest>(configuration.ScrapeRequestTable).Include(x => x.Result);
                return requests.Find(x => x.Status == EScrapeStatus.Accepted).OrderByDescending(x => x.DateReceived).ToImmutableArray();
            }
        }

        public ImmutableArray<ScrapeRequest> FindRequests(Expression<Func<ScrapeRequest, bool>> predicate)
        {
            lock (locker)
            {
                var requests = db.GetCollection<ScrapeRequest>(configuration.ScrapeRequestTable).Include(x => x.Result);
                return requests.Find(predicate).ToImmutableArray();
            }
        }

        public ScrapeRequest FindNextJob()
        {
            lock (locker)
            {
                var requests = db.GetCollection<ScrapeRequest>(configuration.ScrapeRequestTable).Include(x => x.Result);
                var request = requests.Find(x => x.Status == EScrapeStatus.Accepted).OrderByDescending(x => x.DateReceived).FirstOrDefault();

                if (request != null)
                {
                    request.Status = EScrapeStatus.Scraping;
                    UpdateRequest(request);
                }

                return request;
            }
        }

        public ScrapeRequest FindScrapeRequestByGuid(string guid)
        {
            lock (locker)
            {
                var requests = db.GetCollection<ScrapeRequest>(configuration.ScrapeRequestTable).Include(x => x.Result);
                return requests.FindOne(x => x.Id == guid);
            }
        }

        public ImmutableArray<ScrapeRequest> FindScrapeRequestsByUrl(string url)
        {
            lock (locker)
            {
                var requests = db.GetCollection<ScrapeRequest>(configuration.ScrapeRequestTable).Include(x => x.Result);
                return requests.Find(x => x.Url == url).ToImmutableArray();
            }
        }

        public ImmutableArray<ScrapeResult> FindScrapeResultByUrl(string url)
        {
            lock (locker)
            {
                var results = db.GetCollection<ScrapeResult>(configuration.ScrapeResultTable);
                return results.Find(x => x.Url == url).ToImmutableArray();
            }
        }

        public void InsertResult(ScrapeResult result)
        {
            lock (locker)
            {
                var results = db.GetCollection<ScrapeResult>(configuration.ScrapeResultTable);
                results.Insert(result);
            }
        }

        public void UpdateRequest(ScrapeRequest request)
        {
            lock (locker)
            {
                var requests = db.GetCollection<ScrapeRequest>(configuration.ScrapeRequestTable).Include(x => x.Result);
                requests.Update(request);
            }
        }
    }
}
