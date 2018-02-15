using Nancy;
using Newtonsoft.Json;
using Scraper.Database;
using Scraper.Model;

namespace Scraper.Module
{
    public class ScaperModule : NancyModule
    {
        private readonly IDataStore dataStore;

        public ScaperModule(IDataStore dataStore)
        {
            this.dataStore = dataStore;

            SetupRoutes();
        }

        private void SetupRoutes()
        {
            Get["/scrape/"] = _ => ScrapeGithub();

            Get["/scrape/{guid}"] = args => GetScrapeResults(args.guid);
            Post["/scrape/"] = _ => PostNewScrape();

            Get["/scrape/{guid}/status"] = args => RetrieveJobStatus(args.guid);
        }

        private Response GetScrapeResults(string guid)
        {
            return Response.AsText(dataStore.FindScrapeRequestByGuid(guid).Text);
        }

        private Response PostNewScrape()
        {
            var urlsToScrape = JsonConvert.DeserializeObject<PostJsonScape[]>(Request.Body.ToString());
            return Response.AsJson(urlsToScrape);
        }

        private Response RetrieveJobStatus(string guid)
        {
            return Response.AsJson(dataStore.FindScrapeRequestByGuid(guid));
        }

        private Response ScrapeGithub()
        {
            return new Response();
        }
    }
}
