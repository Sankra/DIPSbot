using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Model.BikeShare;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Tests.TestData;
using Xunit;

namespace Tests {
    public class GoogleMapsClientTests {
        readonly GoogleMapsClient googleMapsClient;

        public GoogleMapsClientTests() {
            var configuration = CreateConfig.OfType<IGoogleMapsConfiguration>();
            googleMapsClient = new GoogleMapsClient(configuration, new HttpClient(), new MemoryCache(new MemoryCacheOptions()));
        }

        [Fact]
        public async Task FindBikeSharingStationNearestToAddress() {
            var json = File.ReadAllText("../../../TestData/stations.json");
            var allBikeSharingStations = JsonConvert.DeserializeObject<AllStationsInArea>(json);

            var bikeShareStation = await googleMapsClient.FindBikeSharingStationNearestToAddress("Beddingen 10", allBikeSharingStations);

            Assert.Equal("TMV-odden", bikeShareStation.Name);
        }

        [Fact]
        public async Task CreateImageWithDirections() {
            var bikeStation = Create(63.435399485821023D, 10.409983098506927D);

            var imageUrl = await googleMapsClient.CreateImageWithDirections("Beddingen 10", bikeStation);

            Assert.Contains("https://maps.googleapis.com/maps/api/staticmap?size=600x600&scale=2&maptype=roadmap&region=no&markers=icon:https://hjerpbakk.com/assets/img/person.png%7CBeddingen 10&markers=icon:https://hjerpbakk.com/assets/img/parking.png%3F1%7C63.435399485821,10.4099830985069&path=weight:5%7Ccolor:blue%7Cenc:_tdbK{wp~@yAvDGv@DjAJxABTGz@BhAJT&key=", imageUrl);
        }

        BikeStation Create(double latitude, double longitude) => new BikeStation("", "", 0, 0, latitude, longitude, 0L);
    }
}
