using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BikeshareClient;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Model.BikeSharing;
using Microsoft.Extensions.Caching.Memory;
using Tests.TestData;
using Xunit;

namespace Tests {
    public class BikeSharingClientTests {
        readonly BikeSharingClient bikeSharingClient;

        public BikeSharingClientTests() {
            var configuration = CreateConfig.OfType<IBikeSharingConfiguration>();
            var client = new Client(configuration.BikeSharingApiEndpoint);
            bikeSharingClient = new BikeSharingClient(client, new HttpClient(), new MemoryCache(new MemoryCacheOptions()));
        }

        [Fact]
        public async Task GetAllBikeSharingStations() {
            var allBikeSharingStations = await bikeSharingClient.GetBikeSharingStationsMatchingIntention(Intention.Either);

            Assert.Equal(allBikeSharingStations.BikeSharingStations.Length - 1,
                         Regex.Matches(allBikeSharingStations.PipedCoordinatesToAllStations.Value, "\\|").Count);
            Assert.NotNull(allBikeSharingStations.BikeSharingStations.SingleOrDefault(s => s.Name == "Dokkparken"));
        }
    }
}
