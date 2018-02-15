using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Scraper.Database;
using Scraper.Model;

namespace Scraper.Service
{
    public interface IScrapeRequestService
    {
        /// <summary>
        /// Builds, inserts in to the datastore and updates a list of ScrapeRequests from the raw JSON parsed into a PostJsonScrape
        /// </summary>
        ImmutableArray<ScrapeRequest> BuildRequests(IEnumerable<PostJsonScrape> jsonScrape);
    }

    public class ScrapeRequestService : IScrapeRequestService
    {
        private readonly IDataStore dataStore;

        public ScrapeRequestService(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public ImmutableArray<ScrapeRequest> BuildRequests(IEnumerable<PostJsonScrape> jsonScrape)
        {
            return jsonScrape
                .SelectMany(x => x.URLs.Select(y => TryFindScrapeResultWhenBuildingRequest(y, x.CacheLengthSeconds)))
                .ToImmutableArray();
        }

        private ScrapeRequest TryFindScrapeResultWhenBuildingRequest(string url, double cacheSeconds)
        {
            if (Math.Abs(cacheSeconds - default(double)) < double.Epsilon)
            {
                cacheSeconds = TimeSpan.FromHours(2).TotalSeconds;
            }

            var scrapeResultsByUrl = dataStore.FindScrapeResultByUrl(url);
            var validScrapes = scrapeResultsByUrl.Where(x => cacheSeconds - DateTime.Now.Subtract(x.ScrapedAt).TotalSeconds > 0).ToImmutableArray();

            // always check if we have a result in the db that fits the users cache requirements
            if (validScrapes.Any())
            {
                var scrapeResult = validScrapes.OrderByDescending(x => x.ScrapedAt).FirstOrDefault();
                return BuildRequest(scrapeResult, cacheSeconds);
            }

            // otherwise create a new request, we will patch all the results that have the same URL when we get a result
            return BuildRequest(url, cacheSeconds);
        }

        private static ScrapeRequest BuildRequest(ScrapeResult result, double cacheSeconds)
        {
            var dateRecieved = DateTime.UtcNow;
            var timespanCache = TimeSpan.FromSeconds(cacheSeconds);

            return new ScrapeRequest
            {
                Id = Guid.NewGuid().ToString(),
                Url = result.Url,
                DateReceived = dateRecieved,
                Status = EScrapeStatus.Completed,
                CacheLength = timespanCache,
                CacheExpiresOn = dateRecieved + timespanCache,
                Result = result
            };
        }

        private static ScrapeRequest BuildRequest(string url, double cacheSeconds)
        {
            var request = new ScrapeRequest();
            request.Id = Guid.NewGuid().ToString();

            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                request.Url = url;
                request.Status = EScrapeStatus.Accepted;
            }
            else
            {
                request.Status = EScrapeStatus.Rejected;
            }

            // for the love of god just always use UTC. You can thank me later
            request.DateReceived = DateTime.UtcNow;

            request.CacheLength = TimeSpan.FromSeconds(cacheSeconds);
            request.CacheExpiresOn = request.DateReceived + request.CacheLength;

            return request;
        }
    }
}