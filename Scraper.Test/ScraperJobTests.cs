using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Scraper.Database;
using Scraper.Model;
using Scraper.Scheduler;
using Scraper.Scraper;

namespace Scraper.Test
{
    [TestClass]
    public class ScraperJobTests
    {
        [TestMethod]
        public async Task Ensure_Request_Is_Saved_When_Exception_Is_Thrown()
        {
            var dataStoreMocked = new Mock<IDataStore>();
            var webScraperMock = new Mock<IWebScraper>();

            dataStoreMocked.Setup(x => x.UpdateRequest(It.IsAny<ScrapeRequest>()));
            dataStoreMocked.Setup(x => x.FindRequests(It.IsAny<Expression<Func<ScrapeRequest, bool>>>())).Returns(ImmutableArray<ScrapeRequest>.Empty);
            dataStoreMocked.Setup(x => x.FindNextJob()).Returns(new ScrapeRequest());
            webScraperMock.Setup(x => x.Scrape(It.IsAny<ScrapeRequest>())).Throws<Exception>();

            var scraperJob = new ScraperJob(dataStoreMocked.Object, webScraperMock.Object);
            await scraperJob.Execute(null);

            dataStoreMocked.Verify(x => x.UpdateRequest(It.IsAny<ScrapeRequest>()), Times.Once);
        }

        [TestMethod]
        public async Task Assert_Failed_Request_On_Exception()
        {
            var request = new ScrapeRequest();
            var dataStoreMocked = new Mock<IDataStore>();
            var webScraperMock = new Mock<IWebScraper>();

            dataStoreMocked.Setup(x => x.FindRequests(It.IsAny<Expression<Func<ScrapeRequest, bool>>>())).Returns(ImmutableArray<ScrapeRequest>.Empty);
            dataStoreMocked.Setup(x => x.FindNextJob()).Returns(request);
            webScraperMock.Setup(x => x.Scrape(It.IsAny<ScrapeRequest>())).Throws<Exception>();

            var scraperJob = new ScraperJob(dataStoreMocked.Object, webScraperMock.Object);
            await scraperJob.Execute(null);

            Assert.AreEqual(EScrapeStatus.Failed, request.Status);
        }
    }
}
