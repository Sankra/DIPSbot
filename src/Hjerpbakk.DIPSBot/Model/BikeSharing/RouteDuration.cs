using System.Diagnostics;
using Hjerpbakk.DIPSBot.Model.BikeSharing.Google;

namespace Hjerpbakk.DIPSBot.Model.BikeSharing {
    [DebuggerDisplay("{Duration.Text}", Name = "{Route.From} - {Route.To}")]
    public readonly struct RouteDuration {
        public RouteDuration(in Route route, in Duration duration) {
            Route = route;
            Duration = duration;
        }

        public Route Route { get; }
        public Duration Duration { get; }
    }
}
