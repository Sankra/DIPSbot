using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BikeshareClient.Models;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Model.BikeSharing;
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
        public async Task FindBikeSharingStationsNearestToAddress() {
            var json = File.ReadAllText("../../../TestData/stations.json");
            var stationsTestData = JsonConvert.DeserializeObject<StationsTestData>(json);
            var allStationsInArea = new AllStationsInArea(
                stationsTestData.Stations,
                stationsTestData.Stations.Select(s => new StationStatus(s.Id, 0, 0, 0, 0, 0, DateTime.MinValue)));

            var bikeSharingStations = await googleMapsClient.FindBikeSharingStationsNearestToLocation("Beddingen 10", allStationsInArea);

            Assert.Equal("TMV-odden", bikeSharingStations[0].BikeSharingStation.Name);
            Assert.Equal("Bassengbakken", bikeSharingStations[1].BikeSharingStation.Name);
            Assert.Equal("Dokkparken", bikeSharingStations[2].BikeSharingStation.Name);
        }

        [Fact]
        public async Task CreateImageWithDirections() {
            var bikeStations = Create((63.435399485821023D, 10.409983098506927D),
                                      (63.435920064727952D, 10.414788275957108D),
                                      (63.433509009133566D, 10.411412715911865D));

            var imageUrl = await googleMapsClient.CreateImageWithDirections("Beddingen 10", bikeStations);

            Assert.Contains("https://maps.googleapis.com/maps/api/staticmap?size=600x600&scale=2&maptype=roadmap&region=no&markers=color:green%7Clabel:U%7CBeddingen 10&markers=color:red%7Clabel:A%7C63.435399485821,10.4099830985069&markers=color:red%7Clabel:B%7C63.435920064728,10.4147882759571&markers=color:red%7Clabel:C%7C63.4335090091336,10.4114127159119&path=weight:5%7Ccolor:blue%7Cenc:_tdbK{wp~@yAvDGv@DjAJxABTGz@BhAJT&key=", imageUrl);
        }

        [Fact]
        public async Task GetWalkingDuration() {
            var route = new Route("Beddingen 10", "Lerkendal Stadion", TransportationMode.Walking);

            var routeDuration = await googleMapsClient.GetDuration(route);

            Assert.Equal(2281, routeDuration.Duration.Value);
        }

        [Fact]
        public async Task GetCyclingDuration() {
            var route = new Route("Beddingen 14", "Klæbuveien 125", TransportationMode.Bicycling);

            var routeDuration = await googleMapsClient.GetDuration(route);

            Assert.Equal(775, routeDuration.Duration.Value);
        }

        [Fact]
        public async Task GetDetailedWalkingRoute() {
            var route = new Route("Klæbuveien 125", "Lerkendal Stadion", TransportationMode.Walking);

            var routeSegment = await googleMapsClient.GetDetailedRoute(route);

            Assert.Equal("ca`bK{qn~@mAn@@w@CkACaBk@L?M", routeSegment.Segment.Points);
        }

        [Fact]
        public async Task GetDetailedCyclingRoute() {
            var route = new Route("Beddingen 14", "Klæbuveien 125", TransportationMode.Bicycling);

            var routeSegment = await googleMapsClient.GetDetailedRoute(route);

            Assert.Equal(@"utdbKajp~@r@tASh@Y`B[fBCNKJBhAJpHFjFIbAXRL@fDjC`BrA@MBYBWHEJBPNj@f@^PxAhA~@p@j@f@hBpA~AxAhGhEZh@NPD\APANtCrBpAr@JaBd@mILqCL@v@Yb@Kp@A`AW|BaANI^?x@N`AVRHTTxCrEpB|CxAxB~@nAjBvBvBjC`@X|@XdDpAh@X|At@hAh@p@\z@d@VHVBX?zAe@lB_A`DyAlAq@RMtEkCp@Yl@[l@Y~Aw@nBaAv@a@ZQrAs@NQNGhAk@z@a@lAo@",
                         routeSegment.Segment.Points);
        }

        LabelledBikeSharingStation[] Create(params (double latitude, double longitude)[] coordinates) {
            var labelIndex = 0;
            return coordinates.Select(cord => new LabelledBikeSharingStation(
                                    (char)('A' + labelIndex++),
                                    new BikeSharingStation("", 0, 0, cord.latitude, cord.longitude)))
                              .ToArray();
        }

        public readonly struct StationsTestData {
            [JsonConstructor]
            public StationsTestData(IEnumerable<Station> stations) {
                Stations = stations;
            }

            public IEnumerable<Station> Stations { get; }
        }
    }
}
