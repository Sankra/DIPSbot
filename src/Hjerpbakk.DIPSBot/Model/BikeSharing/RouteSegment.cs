using System.Diagnostics;
using Hjerpbakk.DIPSBot.Model.BikeSharing.Google;

namespace Hjerpbakk.DIPSBot.Model.BikeSharing {
    [DebuggerDisplay("{Segment.Points}", Name = "{Route.From} - {Route.To}")]
    public readonly struct RouteSegment {
        public RouteSegment(in Route route, in Polyline segment) {
            Route = route;
            Segment = segment;
        }

        public Route Route { get; }
        public Polyline Segment { get; }
    }
}
