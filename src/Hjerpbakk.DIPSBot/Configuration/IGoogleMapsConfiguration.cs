namespace Hjerpbakk.DIPSBot.Configuration {
    public interface IGoogleMapsConfiguration : IConfiguration {
        string GoogleMapsApiKey { get; }
        string MapsRegion { get; }
    }
}
