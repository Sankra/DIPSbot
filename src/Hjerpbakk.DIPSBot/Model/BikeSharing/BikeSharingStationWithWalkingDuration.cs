using System;
using System.Diagnostics;

namespace Hjerpbakk.DIPSBot.Model.BikeSharing {
    [DebuggerDisplay("{BikeSharingStation.Address} has {BikeSharingStation.FreeBikes} / {BikeSharingStation.AvailableSpace} bikes", Name = "{BikeSharingStation.Name}")]
    public readonly struct BikeSharingStationWithWalkingDuration {
        public BikeSharingStationWithWalkingDuration(BikeSharingStation bikeSharingStation, Duration walkingDuration) {
            BikeSharingStation = bikeSharingStation ?? throw new ArgumentNullException(nameof(bikeSharingStation));
            WalkingDuration = walkingDuration;
        }

        public BikeSharingStation BikeSharingStation { get; }
        public Duration WalkingDuration { get; }
    }
}
