using System;

namespace Hjerpbakk.DIPSBot.Model.BikeShare {
    public readonly struct BikeShareStationWithWalkingDuration {
        public BikeShareStationWithWalkingDuration(BikeShareStation bikeShareStation, long walkingDuration) {
            BikeShareStation = bikeShareStation ?? throw new ArgumentNullException(nameof(bikeShareStation));
            WalkingDuration = walkingDuration;
        }

        public BikeShareStation BikeShareStation { get; }
        public long WalkingDuration { get; }
    }
}
