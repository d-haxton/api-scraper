using System;

namespace Scraper.Model
{
    public class PostJsonScrape
    {
        public string[] URLs { get; set; }
        public double CacheLengthSeconds { get; set; }

        public PostJsonScrape(string[] urls, double cacheLengthSeconds)
        {
            URLs = urls;

            if (Math.Abs(cacheLengthSeconds - default(double)) < 0.01)
            {
                CacheLengthSeconds = cacheLengthSeconds;
            }
        }
    }
}
