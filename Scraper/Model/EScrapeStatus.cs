using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.Model
{
    public enum EScrapeStatus
    {
        Rejected,
        Accepted,
        Awaiting,
        Scrapping,
        Completed,
        Failed,
    }
}
