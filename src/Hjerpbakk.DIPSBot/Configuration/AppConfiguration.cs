using System;
using Hjerpbakk.DIPSBot.Telemetry;
using Hjerpbakk.ServiceDiscovery.Client.Model;

namespace Hjerpbakk.DIPSBot.Configuration {
    public class AppConfiguration : IReadOnlyAppConfiguration,
                                    IServiceDiscoveryConfiguration,
                                    IGoogleMapsConfiguration,
                                    IImgurConfiguration,
                                    IBikeSharingConfiguration {
        public string SlackAPIToken { get; set; }
        public string AdminUser { get; set; }
        public string BotUser { get; set; }

        public string KitchenServiceURL { get; set; }
        public string ComicsServiceURL { get; set; }

        public Action<Exception> FatalExceptionHandler { get; set; }

        public string ServiceDiscoveryUrl { get; set; }
        public string ApiKey { get; set; }

        public string KitchenResponsibleServiceName { get; set; }
        public string ComicsServiceName { get; set; }

        public string InstrumentationKey { get; set; }

        public Context Context { get; set; }
        public Service Service { get; set; }

        public string GoogleMapsApiKey { get; set; }

        public string ImgurClientId { get; set; }

        public string BikeSharingApiEndpoint { get; set; }
        public string MapsRegion { get; set; }
    }
}
