using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeshareClient.Models;

namespace Hjerpbakk.DIPSBot.Model.BikeSharing {
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

            BikeSharingStations = new BikeSharingStation[stationsArray.Length];
            for (int i = 0; i < stationsArray.Length; i++) {
                BikeSharingStations[i] = new BikeSharingStation(stationsArray[i].Name,
                                                            stationsArray[i].Address,
                                                            stationsStatusArray[i].BikesAvailable,
                                                            stationsStatusArray[i].DocksAvailable,
                                                            stationsArray[i].Latitude,
                                                            stationsArray[i].Longitude);
            }
        }

        public BikeSharingStation[] BikeSharingStations { get; }
        public string PipedCoordinatesToAllStations
            => HttpUtility.UrlEncode(string.Join("|", BikeSharingStations.Select(station => $"{station.Latitude},{station.Longitude}").ToArray()));
    }
}