using Nancy.Bootstrappers.StructureMap;
using Scraper.Scheduler;
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

            var runner = container.GetInstance<IScraperScheduler>();
            runner.Start();
            runner.BuildAndScheduleJob(3);

            base.ConfigureApplicationContainer(container);
        }
    }
}
