using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Scraper.Database;
using Scraper.Model;
using Scraper.Scraper;
using Shouldly;

namespace Scraper.Test
{
    [TestClass]
    public class WebScraperTests
    {
        [TestMethod]
        public void SomeTest()
        {
            var request = new ScrapeRequest();

            var dataStoreMocked = new Mock<IDataStore>();
            var httpClientMocked = new Mock<IHttpClientWrapper>();

            httpClientMocked.SetupAllProperties();

            var webScrapper = new WebScraper(dataStoreMocked.Object, httpClientMocked.Object);

            Should.ThrowAsync<HttpRequestException>(async () => { await webScrapper.Scrape(request); });
        }
    }
}
