using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BikeshareClient;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Model.BikeShare;
using Microsoft.Extensions.Caching.Memory;
using Tests.TestData;
using Xunit;

namespace Tests {
    public class BikeShareClientTests {
        readonly BikeShareClient bikeShareClient;

        public BikeShareClientTests() {
            var configuration = CreateConfig.OfType<IBikeShareConfiguration>();
            var client = new Client(configuration.BikeShareApiEndpoint);
            bikeShareClient = new BikeShareClient(client, new HttpClient(), new MemoryCache(new MemoryCacheOptions()));
        }

        [Fact]
        public async Task GetAllBikeSharingStations() {
            var allBikeSharingStations = await bikeShareClient.GetAllBikeSharingStations(Intention.Either);

            Assert.Equal(allBikeSharingStations.BikeShareStations.Length - 1,
                         Regex.Matches(allBikeSharingStations.PipedCoordinatesToAllStations, "%7c").Count);
            Assert.NotNull(allBikeSharingStations.BikeShareStations.SingleOrDefault(s => s.Name == "Dokkparken"));
        }
    }
}
