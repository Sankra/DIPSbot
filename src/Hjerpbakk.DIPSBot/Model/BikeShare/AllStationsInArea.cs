using System;
using System.Collections.Generic;
using BikeshareClient.Models;

namespace Hjerpbakk.DIPSBot.Model.BikeShare {
    public readonly struct AllStationsInArea {
        public AllStationsInArea(Station[] stations, StationStatus[] stationsStatus, string pipedCoordinatesToAllStations) {
            if (stations == null) {
                throw new ArgumentNullException(nameof(stations));
            }

            if (stationsStatus == null) {
                throw new ArgumentNullException(nameof(stationsStatus));
            }

            if (stations.Length != stationsStatus.Length) {
                throw new ArgumentException($"{nameof(stations)} length ({stations.Length}) must be equal to {nameof(stationsStatus)} length {stationsStatus.Length}.");
            }

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
