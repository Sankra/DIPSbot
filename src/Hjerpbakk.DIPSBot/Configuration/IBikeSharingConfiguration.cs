namespace Hjerpbakk.DIPSBot.Configuration {
    public interface IBikeSharingConfiguration : IConfiguration {
        string BikeSharingApiEndpoint { get; }
    }
}
