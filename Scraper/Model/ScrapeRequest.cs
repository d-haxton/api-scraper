using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Scraper.Model
{
    public class ScrapeRequest : ChildObject
    {
        public string Url { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime DateCompleted { get; set; }
        public DateTime CacheExpiresOn { get; set; }
        public TimeSpan CacheLength { get; set; }

        [IgnoreDataMember]
        public ScrapeResult Result { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EScrapeStatus Status { get; set; }
    }
}
