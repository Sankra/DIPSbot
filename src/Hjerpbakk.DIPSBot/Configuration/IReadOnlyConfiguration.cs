using System;
using Hjerpbakk.ServiceDiscovery.Client.Model;

namespace Hjerpbakk.DIPSBot.Configuration {
    public interface IReadOnlyAppConfiguration : IConfiguration {
        string SlackAPIToken { get; }
        string AdminUser { get; }
        string BotUser { get; }

        string KitchenServiceURL { get; }
        string ComicsServiceURL { get; set; }

        Action<Exception> FatalExceptionHandler { get; }
        Service Service { get; }
    }
}
