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
        public async Task GetQuickestRoute() {
            const string From = "Beddingen 10";
            const string To = "Lerkendal Stadion";

            var result = await bikeSharingService.GetQuickestRoute(From, To);

            Assert.Equal("🚲 The quickest route from Beddingen 10 to Lerkendal Stadion will take 18 mins. You should walk from Beddingen 10 to TMV-odden (2 mins), bicycle to Lerkendal (13 mins) and walk to Lerkendal Stadion (2 mins).", result.Directions);
        }

        [Fact]
        public async Task GetQuickestRoute2() {
            const string From = "Kolstadflata 13A";
            const string To = "Beddingen 10";

            var result = await bikeSharingService.GetQuickestRoute(From, To);

            Assert.Equal("🚲 The quickest route from Kolstadflata 13A to Beddingen 10 will take 1 hour 30 mins. You should walk from Kolstadflata 13A to Lerkendal (1 hour 16 mins), bicycle to TMV-odden (10 mins) and walk to Beddingen 10 (3 mins).", result.Directions);
        }

        [Fact]
        public async Task GetQuickestRoute3() {
            const string From = "Beddingen 10";
            const string To = "Olavshallen";

            var result = await bikeSharingService.GetQuickestRoute(From, To);

            Assert.Equal("🚶 You can walk directly from Beddingen 10 to Olavshallen in 7 mins.", result.Directions);
        }

        [Fact]
        public async Task GetQuickestRoute4() {
            const string From = "Oslo";
            const string To = "Beddingen 10";

            var result = await bikeSharingService.GetQuickestRoute(From, To);

            Assert.Equal("🚲 The quickest route from Oslo to Beddingen 10 will take 4 days 7 hours. You should walk from Oslo to Lerkendal (4 days 6 hours), bicycle to TMV-odden (10 mins) and walk to Beddingen 10 (3 mins).", result.Directions);
        }

        [Fact]
        public async Task GetQuickestRoute5() {
            const string From = "Kolstadflata 10";
            const string To = "Olavshallen";

            var result = await bikeSharingService.GetQuickestRoute(From, To);

            Assert.Equal("🚲 The quickest route from Kolstadflata 10 to Olavshallen will take 1 hour 25 mins. You should walk from Kolstadflata 10 to Lerkendal (1 hour 13 mins), bicycle to Bakke bru (8 mins) and walk to Olavshallen (3 mins).", result.Directions);
        }
    }
}
