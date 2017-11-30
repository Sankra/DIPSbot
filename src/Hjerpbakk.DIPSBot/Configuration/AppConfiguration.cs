using System;
using Hjerpbakk.DIPSBot.Telemetry;

namespace Hjerpbakk.DIPSBot.Configuration
{
    public class AppConfiguration : IReadOnlyAppConfiguration
    {
        public string SlackAPIToken { get; set; }
        public string AdminUser { get; set; }
        public string BotUser { get; set; }

        public string KitchenServiceURL { get; set; }
        public string ComicsServiceURL { get; set; }

        public Action<Exception> FatalExceptionHandler { get; set; }

        public string ServiceDiscoveryURL { get; set; }
        public string KitchenResponsibleServiceName { get; set; }
        public string ComicsServiceName { get; set; }

        public string InstrumentationKey { get; set; }

        public Context Context { get; set; }
    }
}
