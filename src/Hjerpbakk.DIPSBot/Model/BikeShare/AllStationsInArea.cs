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

            BikeShareStations = new BikeShareStation[stationsArray.Length];
            for (int i = 0; i < stationsArray.Length; i++) {
                BikeShareStations[i] = new BikeShareStation(stationsArray[i].Name,
                                                            stationsArray[i].Address,
                                                            stationsStatusArray[i].BikesAvailable,
                                                            stationsStatusArray[i].DocksAvailable,
                                                            stationsArray[i].Latitude,
                                                            stationsArray[i].Longitude);
            }
        }

        public BikeShareStation[] BikeShareStations { get; }
        public string PipedCoordinatesToAllStations
            => HttpUtility.UrlEncode(string.Join("|", BikeShareStations.Select(station => $"{station.Latitude},{station.Longitude}").ToArray()));
    }
}