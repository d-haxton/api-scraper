using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nancy;
using Nancy.Testing;
using Newtonsoft.Json;
using Scraper.Database;
using Scraper.Model;
using Scraper.Module;
using Scraper.Service;

namespace Scraper.Test
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public void Can_Post_Json_To_Nancy()
        {
            var postJson = new PostJsonScrape(new[] {"https://google.com"}, 30);
            var jsonArray = new [] {postJson};
            var json = JsonConvert.SerializeObject(jsonArray);

            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            // When
            var result = browser.Post("/scrape/", with => {
                with.HttpRequest();
                with.Body(json);
            });

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void Can_Get_Json_From_Nancy()
        {
            var postJson = new PostJsonScrape(new[] { "https://google.com" }, 30);
            var jsonArray = new[] { postJson };
            var json = JsonConvert.SerializeObject(jsonArray);

            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            var returnResult = browser.Post("/scrape/", with => {
                with.HttpRequest();
                with.Body(json);
            });

            var scrapeRequests = returnResult.Body.DeserializeJson<List<ScrapeRequest>>();

            var result = browser.Get($"/scrape/{scrapeRequests[0].Id}/status", with => {
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void Ensure_No_Content_When_Asking()
        {
            var postJson = new PostJsonScrape(new[] { "https://google.com" }, 30);
            var jsonArray = new[] { postJson };
            var json = JsonConvert.SerializeObject(jsonArray);

            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            var returnResult = browser.Post("/scrape/", with => {
                with.HttpRequest();
                with.Body(json);
            });

            var scrapeRequests = returnResult.Body.DeserializeJson<List<ScrapeRequest>>();

            var result = browser.Get($"/scrape/{scrapeRequests[0].Id}", with => {
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [TestMethod]
        public void Return_Not_Found_When_Asking_For_Empty_Guid()
        {
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            var result = browser.Get($"/scrape/1", with => {
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}
