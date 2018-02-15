using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.Model
{
    public class PostJsonScape
    {
        public string[] URLs { get; set; }
        public int CacheLengthSeconds { get; set; }

        public PostJsonScape(string[] urLs, int cacheLengthSeconds)
        {
            URLs = urLs;
            CacheLengthSeconds = cacheLengthSeconds;
        }
    }
}
