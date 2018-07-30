using Newtonsoft.Json;

namespace Hjerpbakk.DIPSBot.Model.BikeSharing {
    public readonly struct Route {
        [JsonConstructor]
        public Route(RouteElement[] routes, string status) {
            Routes = routes;
            Status = status;
        }

        public RouteElement[] Routes { get; }
        public string Status { get; }
    }

    public readonly struct RouteElement {
        [JsonConstructor]
        public RouteElement([JsonProperty("overview_polyline")] Polyline overviewPolyline) {
            OverviewPolyline = overviewPolyline;
        }

        public Polyline OverviewPolyline { get; }
    }

    public readonly struct Polyline {
        [JsonConstructor]
        public Polyline(string points) {
            Points = points;
        }

        public string Points { get; }
    }
}
