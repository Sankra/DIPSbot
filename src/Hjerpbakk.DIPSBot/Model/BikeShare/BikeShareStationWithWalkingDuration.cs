using System;
using System.Diagnostics;

namespace Hjerpbakk.DIPSBot.Model.BikeShare {
    [DebuggerDisplay("{BikeShareStation.Address} has {BikeShareStation.FreeBikes} / {BikeShareStation.AvailableSpace} bikes", Name = "{BikeShareStation.Name}")]
    public readonly struct BikeShareStationWithWalkingDuration {
        public BikeShareStationWithWalkingDuration(BikeShareStation bikeShareStation, long walkingDuration) {
            BikeShareStation = bikeShareStation ?? throw new ArgumentNullException(nameof(bikeShareStation));
            WalkingDuration = walkingDuration;
        }

        public BikeShareStation BikeShareStation { get; }
        public long WalkingDuration { get; }
    }
}
