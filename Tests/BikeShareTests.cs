using System.Net.Http;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Clients;
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
        TrondheimBysykkelClient CreateClient() => new TrondheimBysykkelClient(new HttpClient(),
                                                                              new MemoryCache(new MemoryCacheOptions()),
                                                                              "");
    }
}
