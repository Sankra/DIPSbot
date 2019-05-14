using System;
using System.Collections.Generic;
using System.Linq;
using BikeshareClient.Models;

namespace Hjerpbakk.DIPSBot.Model.BikeSharing {
    public readonly struct AllStationsInArea {
        public AllStationsInArea(in IEnumerable<Station> stations, in IEnumerable<StationStatus> stationsStatus) {
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

            // TODO: Match on ID, skip status for stations where status does not exist

            // stationsTask.Result.Join(stationsStatusTask.Result,
            //                                 station => station.Id,
            //                                 stationStatus => stationStatus.Id,
            //                                 (station, stationStatus) => new BikeSharingStation()).ToArray();

            BikeSharingStations = new BikeSharingStation[stationsArray.Length];
            for (int i = 0; i < stationsArray.Length; i++) {
                BikeSharingStations[i] = new BikeSharingStation(stationsArray[i].Name,
                                                            stationsStatusArray[i].BikesAvailable,
                                                            stationsStatusArray[i].DocksAvailable,
                                                            stationsArray[i].Latitude,
                                                            stationsArray[i].Longitude);
            }
        }

        public BikeSharingStation[] BikeSharingStations { get; }
        public Address PipedCoordinatesToAllStations
            => new Address(string.Join("|", BikeSharingStations.Select(station => station.Address.Value).ToArray()));
    }
}