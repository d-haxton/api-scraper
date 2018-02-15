using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Scraper.Database;
using Scraper.Model;
using Scraper.Scraper;

namespace Scraper.Scheduler
{
    public class ScraperJob : IJob
    {
        private readonly IDataStore dataStore;
        private readonly IWebScraper webScraper;

        public ScraperJob(IDataStore dataStore, IWebScraper webScraper)
        {
            this.dataStore = dataStore;
            this.webScraper = webScraper;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            ScrapeRequest scrapeRequest = null;
            ScrapeResult result = null;
            try
            {
                scrapeRequest = dataStore.FindNextJob();
                if (scrapeRequest != null)
                {
                    result = await webScraper.Scrape(scrapeRequest);
                }
            }
            catch
            {
                if (scrapeRequest != null)
                {
                    // if we were doing this for real we could just retry this with exponential back off
                    scrapeRequest.Status = EScrapeStatus.Failed;
                    dataStore.UpdateRequest(scrapeRequest);
                }
            }
            finally
            {
                if (scrapeRequest != null)
                {
                    dataStore.UpdateRequest(scrapeRequest);
                    UpdateAnyPendingRequestsOnSameUrl(scrapeRequest, result);
                }
            }
        }

        /// <summary>
        /// Iterate through the datastore finding requests that have been accepted but not queued which URL matches the URL of the request this just proccessed.
        /// This is to avoid hitting the same URL multiple times because of a failed URL call will not have a ScrapeResult therefore if you batched it 100x times it would
        /// continue to hit and ask for it. Considering we allow a cache of 0.1 seconds it doesn't make sense to avoid hitting these links in the future.
        /// </summary>
        private void UpdateAnyPendingRequestsOnSameUrl(ScrapeRequest request, ScrapeResult result)
        {
            foreach (var scrapeRequest in dataStore.FindRequests(x => x.Url == request.Url && x.Id != request.Id && x.Status == EScrapeStatus.Accepted))
            {
                scrapeRequest.Result = result;
                scrapeRequest.Status = EScrapeStatus.Completed;
                dataStore.UpdateRequest(scrapeRequest);
            }
        }
    }
}
