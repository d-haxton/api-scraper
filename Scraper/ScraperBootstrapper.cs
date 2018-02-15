using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrappers.StructureMap;
using Scraper.Structuremap;
using StructureMap;
using StructureMap.Graph;

namespace Scraper
{
    public sealed class ScraperBootstrapper : StructureMapNancyBootstrapper
    {
        public ScraperBootstrapper()
        {
#if DEBUG
            DiagnosticsConfiguration.Password = "password";
#endif
        }

        protected override void ConfigureApplicationContainer(IContainer container)
        {
            container.Configure(x =>
            {
                x.AddRegistry(new ScraperRegistry());

                x.Scan(scanner =>
                {
                    scanner.WithDefaultConventions();
                    scanner.TheCallingAssembly();
                });
            });
            base.ConfigureApplicationContainer(container);
        }

    }
}
