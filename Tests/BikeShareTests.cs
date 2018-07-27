using System.Net.Http;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace Tests {
    public class BikeShareTests {
        [Fact]
        public async Task StationNearestGløsIsReturnedCorrectly() {
            var client = CreateClient();

            var bikeShareStation = await client.FindNearesBikeSharingStation("Beddingen 10");

            Assert.Equal("TMV-odden", bikeShareStation.Name);
        }

        // TODO: How to use API key in tests while not checking it in?
        TrondheimBysykkelClient CreateClient() {
            var googleMapsConfiguration = new AppConfiguration {
                GoogleMapsApiKey = ""
            };
            return new TrondheimBysykkelClient(new HttpClient(),
                                               googleMapsConfiguration,
                                               new MemoryCache(new MemoryCacheOptions()));
        }
    }
}
