using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BikeshareClient;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
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
            var allBikeSharingStations = await bikeShareClient.GetAllBikeSharingStations();

            File.WriteAllText("/Users/sankra/Downloads/stations.json", JsonConvert.SerializeObject(allBikeSharingStations));

            var orderedBikeSharingStations = allBikeSharingStations.Stations.OrderBy(s => s.Id);
            var firstBikeSharingStation = orderedBikeSharingStations.First();
            var orderedBikeSharingStatus = allBikeSharingStations.StationsStatus.OrderBy(s => s.Id);
            Assert.Equal(allBikeSharingStations.Stations.Length - 1,
                         Regex.Matches(allBikeSharingStations.PipedCoordinatesToAllStations, "%7c").Count);
            Assert.Equal("1", firstBikeSharingStation.Id);
            Assert.Equal("Dokkparken", firstBikeSharingStation.Name);
            Assert.Equal("1", orderedBikeSharingStatus.First().Id);
        }
    }
}
