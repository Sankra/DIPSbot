namespace Hjerpbakk.DIPSBot.Configuration {
    public interface IServiceDiscoveryConfiguration : IConfiguration {
        string ServiceDiscoveryUrl { get; }
        string ApiKey { get; }
    }
}
