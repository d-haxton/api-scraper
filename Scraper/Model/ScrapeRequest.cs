using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.Model
{
    public class ScrapeRequest
    {
        public string Guid { get; set; }
        public Uri Uri { get; set; }
        public EScrapeStatus Status { get; set; }
        public string Text { get; set; }
        public DateTime DateAccepted { get; set; }
        public DateTime DateCompleted { get; set; }
        public DateTime CacheExpiresOn { get; set; }
        public TimeSpan CacheLength { get; set; }
    }
}
