using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.Scraper
{
    public interface IHttpClientWrapper
    {
        HttpClient Client { get; }
    }

    public class HttpClientWrapper : IHttpClientWrapper
    {
        // don't dispose httpclient: https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        public HttpClient Client { get; } = new HttpClient();
    }
}
