using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Scraper.Database;
using Scraper.Model;

namespace Scraper.Scraper
{
    public interface IWebScraper
    {
        /// <summary>
        /// Hits a URL. Downloads its contents and builds a ScrapeResult from the content
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The result containing the text and when it was scraped at</returns>
        Task<ScrapeResult> Scrape(ScrapeRequest request);
    }

    public class WebScraper : IWebScraper
    {
        private readonly IDataStore dataStore;

        // don't dispose httpclient: https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        private static readonly HttpClient Client = new HttpClient();

        public WebScraper(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public async Task<ScrapeResult> Scrape(ScrapeRequest request)
        {
            var response = await Client.GetAsync(request.Url);
            response.EnsureSuccessStatusCode();
            using (var content = response.Content)
            {
                var stringResult = await content.ReadAsStringAsync();
                return BuildAndAddResult(stringResult, ref request);
            }
        }


        private ScrapeResult BuildAndAddResult(string text, ref ScrapeRequest request)
        {
            var result = new ScrapeResult
            {
                Id = Guid.NewGuid().ToString(),
                ScrapedAt = DateTime.UtcNow,
                Text = text,
                Url = request.Url
            };

            request.DateCompleted = DateTime.UtcNow;
            request.Status = EScrapeStatus.Completed;
            request.Result = result;

            dataStore.InsertResult(result);

            return result;
        }
    }
}
