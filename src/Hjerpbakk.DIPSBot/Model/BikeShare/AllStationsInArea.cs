using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeshareClient.Models;

namespace Hjerpbakk.DIPSBot.Model.BikeShare {
    public readonly struct AllStationsInArea {
        public AllStationsInArea(IEnumerable<Station> stations, IEnumerable<StationStatus> stationsStatus) {
            if (stations == null) {
                throw new ArgumentNullException(nameof(stations));
            }

            if (stationsStatus == null) {
                throw new ArgumentNullException(nameof(stationsStatus));
            }

            var stationsArray = stations.ToArray();
            var stationsStatusArray = stationsStatus.ToArray();
            if (stationsArray.Length != stationsStatusArray.Length) {
                throw new ArgumentException($"{nameof(stations)} length ({stationsArray.Length}) must be equal to {nameof(stationsStatus)} length {stationsStatusArray.Length}.");
            }

            Stations = stationsArray;
            StationsStatus = stationsStatusArray;
        }

        // TODO: Is ugly
        public Station[] Stations { get; }
        public StationStatus[] StationsStatus { get; }
        public string PipedCoordinatesToAllStations
            => HttpUtility.UrlEncode(string.Join("|", Stations.Select(station => $"{station.Latitude},{station.Longitude}").ToArray()));
    }
}