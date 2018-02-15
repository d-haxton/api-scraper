using System.Net.Http;

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
