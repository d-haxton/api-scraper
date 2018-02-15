using System.Runtime.Serialization;

namespace Scraper.Model
{
    public enum EScrapeStatus
    {
        [EnumMember(Value = "rejected")]
        Rejected,

        [EnumMember(Value = "accepted")]
        Accepted,

        [EnumMember(Value = "scraping")]
        Scraping,

        [EnumMember(Value = "completed")]
        Completed,

        [EnumMember(Value = "failed")]
        Failed,
    }
}
