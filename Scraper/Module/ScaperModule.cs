using Nancy;
using Newtonsoft.Json;
using Scraper.Database;
using Scraper.Model;
using Scraper.Service;

namespace Scraper.Module
{
    public class ScaperModule : NancyModule
    {
        private readonly IScrapeRequestService scrapeRequestService;
        private readonly IDataStore dataStore;

        public ScaperModule(IScrapeRequestService scrapeRequestService, IDataStore dataStore)
        {
            this.scrapeRequestService = scrapeRequestService;
            this.dataStore = dataStore;

            SetupRoutes();
        }

        private void SetupRoutes()
        {
            Get["/scrape/{guid}/status"] = args => RetrieveJobStatus(args["guid"]);
            Get["/scrape/{guid}"] = args => GetScrapeResults(args["guid"]);

            Post["/scrape/"] = _ => PostNewScrape();

            Get["/jobs/"] = _ => GetAllNonProccessedRequests();
        }

        /// <summary>
        /// Queries the datastore for all Accepted jobs
        /// </summary>
        private Response GetAllNonProccessedRequests()
        {
            var requests = dataStore.FindAllJobs();
            return Response.AsJson(requests);
        }

        /// <summary>
        /// Returns the ScrapeRequest's Result by guid
        /// </summary>
        private Response GetScrapeResults(string guid)
        {
            var request = dataStore.FindScrapeRequestByGuid(guid);
            if (request == null)
            {
                return HttpStatusCode.NotFound;
            }

            if (request.Result == null)
            {
                return HttpStatusCode.NoContent;
            }

            return Response.AsText(dataStore.FindScrapeRequestByGuid(guid).Result.Text);
        }

        /// <summary>
        /// Parses a list of PostJsonScrape which allows you to batch queue URLs by a cache length
        /// </summary>
        private Response PostNewScrape()
        {
            var json = Nancy.Extensions.RequestStreamExtensions.AsString(Request.Body);
            var urlsToScrape = JsonConvert.DeserializeObject<PostJsonScrape[]>(json);
            var requests = scrapeRequestService.BuildRequests(urlsToScrape);

            dataStore.AddNewScrapeRequest(requests);
            return Response.AsJson(requests);
        }

        /// <summary>
        /// Simply returns the ScrapeRequest object for a specific guid
        /// </summary>
        /// <param name="guid"></param>
        private Response RetrieveJobStatus(string guid)
        {
            var request = dataStore.FindScrapeRequestByGuid(guid);
            if (request == null)
            {
                return HttpStatusCode.NotFound;
            }

            return Response.AsJson(request);
        }
    }
}
