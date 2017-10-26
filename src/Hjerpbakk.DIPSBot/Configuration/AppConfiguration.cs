using System;
using Newtonsoft.Json;

namespace Hjerpbakk.DIPSBot.Configuration
{
    public class AppConfiguration : IReadOnlyAppConfiguration
    {
        public string SlackAPIToken { get; set; }
        public string AdminUser { get; set;  }
        public string BotUser { get; set; }
		 
        public string KitchenServiceURL { get; set; }
        public string ComicsServiceURL { get; set; }

        public Action<Exception> FatalExceptionHandler { get; set; }

        public string ServiceDiscoveryServerName { get; set; }
        public string KitchenResponsibleServiceName { get; set; }
        public string ComicsServiceName { get; set; }
    }
}
