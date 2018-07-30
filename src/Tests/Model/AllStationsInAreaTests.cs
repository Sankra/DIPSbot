using System;
using System.Linq;
using BikeshareClient.Models;
using Hjerpbakk.DIPSBot.Model.BikeShare;
using Xunit;

namespace Tests.Model {
    public class AllStationsInAreaTests {
        [Fact]
        public void CTOR() {
            var original = new AllStationsInArea(
                new[] { new Station("1", "1", "", 0D, 0D, 0), new Station("3", "3", "", 0D, 0D, 0) },
                new[] { new StationStatus("1", 7, 0, 0, 0, 0, DateTime.MinValue), new StationStatus("3", 7, 7, 0, 0, 0, DateTime.MinValue) });

            Assert.Equal("0%2c0%7c0%2c0", original.PipedCoordinatesToAllStations);
            Assert.Equal(2, original.BikeShareStations.Length);
            Assert.Equal("1", original.BikeShareStations[0].Name);
            Assert.Equal("3", original.BikeShareStations[1].Name);

            var filtered = new AllStationsInArea(original.BikeShareStations.Where(s => s.AvailableSpace > 0));

            Assert.Equal("0%2c0", filtered.PipedCoordinatesToAllStations);
            Assert.Single(filtered.BikeShareStations);
            Assert.Equal("3", filtered.BikeShareStations[0].Name);
        }
    }
}
