namespace Hjerpbakk.DIPSBot.Configuration {
    public interface IBikeShareConfiguration : IConfiguration {
        string BikeShareApiEndpoint { get; }
    }
}
