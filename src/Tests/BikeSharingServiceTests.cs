using System;
using System.Net.Http;
using System.Threading.Tasks;
using BikeshareClient;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Services;
using Microsoft.Extensions.Caching.Memory;
using Tests.TestData;
using Xunit;

namespace Tests {
    public class BikeSharingServiceTests {
        readonly BikeSharingService bikeSharingService;

        public BikeSharingServiceTests() {
            // TODO: What a mess. Composition Root for test FTW?
            var config = CreateConfig.OfType<AppConfiguration>();
            var bikeShareClient = new Client(config.BikeSharingApiEndpoint);
            var httpClient = new HttpClient();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var bikeSharingClient = new BikeSharingClient(bikeShareClient, httpClient, memoryCache);
            var googleMapsClient = new GoogleMapsClient(config, httpClient, memoryCache);
            var imgurClient = new ImgurClient(config, httpClient);
            bikeSharingService = new BikeSharingService(bikeSharingClient, googleMapsClient, imgurClient);
        }

        [Fact]
        public async Task GetFastestRoute() {
            const string From = "Beddingen 10";
            const string To = "Lerkendal Stadion";

            var result = await bikeSharingService.GetQuickestRoute(From, To);
        }
    }
}
