using System.Net.Http;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Model.BikeShare;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace Tests {
    public class BikeShareTests {
        readonly TrondheimBysykkelClient client;

        public BikeShareTests() {
            client = CreateClient();
        }

        [Fact]
        public async Task StationNearestOfficeIsReturnedCorrectly() {
            var bikeShareStation = await client.FindNearesBikeSharingStation("Beddingen 10");

            Assert.Equal("TMV-odden", bikeShareStation.Name);
        }

        [Fact]
        public async Task ImageWithRouteIsCorrect() {
            var bikeStation = Create(63.435399485821023D, 10.409983098506927D);

            var imageUrl = await client.FindDirectionsImage("Beddingen 10", bikeStation);

            Assert.Equal("https://maps.googleapis.com/maps/api/staticmap?size=600x600&scale=2&maptype=roadmap&region=no&markers=icon:https://hjerpbakk.com/assets/img/person.png%7CBeddingen 10&markers=icon:https://hjerpbakk.com/assets/img/parking.png%3F1%7C63.435399485821,10.4099830985069&path=weight:5%7Ccolor:blue%7Cenc:_tdbK{wp~@yAvDGv@DjAJxABTGz@BhAJT&key=", imageUrl);
        }

        TrondheimBysykkelClient CreateClient() {
            var googleMapsConfiguration = new AppConfiguration {
                GoogleMapsApiKey = ""
            };
            return new TrondheimBysykkelClient(new HttpClient(),
                                               googleMapsConfiguration,
                                               new MemoryCache(new MemoryCacheOptions()));
        }

        BikeStation Create(double latitude, double longitude) => new BikeStation("", "", 0, 0, latitude, longitude, 0L);
    }
}
