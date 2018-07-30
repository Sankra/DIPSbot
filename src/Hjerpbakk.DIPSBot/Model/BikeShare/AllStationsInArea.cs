using System.Collections.Generic;
using BikeshareClient.Models;

namespace Hjerpbakk.DIPSBot.Model.BikeShare {
    public readonly struct AllStationsInArea {
        public AllStationsInArea(Station[] stations, IEnumerable<StationStatus> stationsStatus, string pipedCoordinatesToAllStations) {
            Stations = stations;
            StationsStatus = stationsStatus;
            PipedCoordinatesToAllStations = pipedCoordinatesToAllStations;
        }

        // TODO: Is ugly
        public Station[] Stations { get; }
        public IEnumerable<StationStatus> StationsStatus { get; }
        public string PipedCoordinatesToAllStations { get; }
    }
}
