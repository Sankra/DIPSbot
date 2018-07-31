using System.Net.Http;
using System.Threading.Tasks;
using BikeshareClient;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Model.BikeSharing;
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
        public async Task GetInformationOnNearestBikeSharingStations() {
            const string Location = "Beddingen 10";

            var result = await bikeSharingService.GetInformationOnNearestBikeSharingStations(Location, Intention.Either);

            Assert.Equal(3, result.Response.Length);
            Assert.Equal("TMV-odden (A) has 12 free bikes / 16 free locks. Estimated walking time is 2 mins.", result.Response[0]);
            Assert.Equal("Bassengbakken (B) has 14 free bikes / 14 free locks. Estimated walking time is 2 mins.", result.Response[1]);
            Assert.Equal("Dokkparken (C) has 4 free bikes / 36 free locks. Estimated walking time is 3 mins.", result.Response[2]);
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

            Assert.Equal("🚲 The quickest route from Beddingen 10 to Olavshallen will take 6 mins. You should walk from Beddingen 10 to TMV-odden (2 mins), bicycle to Verftsbrua (1 min) and walk to Olavshallen (2 mins).", result.Directions);
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

            Assert.Equal("🚲 The quickest route from Kolstadflata 10 to Olavshallen will take 1 hour 26 mins. You should walk from Kolstadflata 10 to Lerkendal (1 hour 13 mins), bicycle to Verftsbrua (10 mins) and walk to Olavshallen (2 mins).", result.Directions);
        }

        [Fact]
        public async Task GetQuickestRoute6() {
            const string From = "Tyholttårnet";
            const string To = "Kristiansten Festning";

            var result = await bikeSharingService.GetQuickestRoute(From, To);

            Assert.Equal("🚶 You can walk directly from Tyholttårnet to Kristiansten Festning in 17 mins.", result.Directions);
        }
    }
}
