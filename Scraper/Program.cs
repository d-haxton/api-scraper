using System;
using Nancy.Hosting.Self;

namespace Scraper
{
    public static class Program
    {
        public static void Main(string[] args)
        {

            using (var nancyHost = new NancyHost(new Uri("http://localhost:8888/nancy/")))
            {
                nancyHost.Start();

                Console.WriteLine("Nancy now listening - navigating to http://localhost:8888/nancy/. Press enter to stop");
                Console.ReadLine();
            }
        }
    }
}
