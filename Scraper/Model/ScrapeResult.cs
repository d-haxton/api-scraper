using System;

namespace Scraper.Model
{
    public class ScrapeResult : ChildObject
    {
        public string Text { get; set; }
        public DateTime ScrapedAt { get; set; }
        public string Url { get; set; }
    }
}
