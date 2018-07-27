using System.Collections.Generic;
using BikeshareClient.Models;

namespace Hjerpbakk.DIPSBot.Model.BikeShare {
    public readonly struct AllStationsInArea {
        public AllStationsInArea(Station[] stations, string coordinates, IEnumerable<StationStatus> stationsStatus) {
            Stations = stations;
            Coordinates = coordinates;
            StationsStatus = stationsStatus;
        }

        public Station[] Stations { get; }
        public string Coordinates { get; }
        public IEnumerable<StationStatus> StationsStatus { get; }
    }
}
