using System.Diagnostics;
using Hjerpbakk.DIPSBot.Model.BikeSharing.Google;

namespace Hjerpbakk.DIPSBot.Model.BikeSharing {
    [DebuggerDisplay("{BikeSharingStation.Address} has {BikeSharingStation.FreeBikes} / {BikeSharingStation.AvailableSpace} bikes", Name = "{BikeSharingStation.Name}")]
    public readonly struct BikeSharingStationWithWalkingDuration {
        public BikeSharingStationWithWalkingDuration(in BikeSharingStation bikeSharingStation, in Duration walkingDuration) {
            BikeSharingStation = bikeSharingStation;
            WalkingDuration = walkingDuration;
        }

        public BikeSharingStation BikeSharingStation { get; }
        public Duration WalkingDuration { get; }
    }
}
