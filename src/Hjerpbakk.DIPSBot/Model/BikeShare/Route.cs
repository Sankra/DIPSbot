using Newtonsoft.Json;

namespace Hjerpbakk.DIPSBot.Model.BikeShare {
    // TODO: struct and readonlyfi
    public partial class Route {
        [JsonProperty("geocoded_waypoints")]
        public GeocodedWaypoint[] GeocodedWaypoints { get; set; }

        [JsonProperty("routes")]
        public RouteElement[] Routes { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public partial class GeocodedWaypoint {
        [JsonProperty("geocoder_status")]
        public string GeocoderStatus { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("types")]
        public string[] Types { get; set; }
    }

    public partial class RouteElement {
        [JsonProperty("bounds")]
        public Bounds Bounds { get; set; }

        [JsonProperty("copyrights")]
        public string Copyrights { get; set; }

        [JsonProperty("legs")]
        public Leg[] Legs { get; set; }

        [JsonProperty("overview_polyline")]
        public Polyline OverviewPolyline { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("warnings")]
        public object[] Warnings { get; set; }

        [JsonProperty("waypoint_order")]
        public object[] WaypointOrder { get; set; }
    }

    public partial class Bounds {
        [JsonProperty("northeast")]
        public Northeast Northeast { get; set; }

        [JsonProperty("southwest")]
        public Northeast Southwest { get; set; }
    }

    public partial class Northeast {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

    public partial class Leg {
        [JsonProperty("distance")]
        public Distance Distance { get; set; }

        [JsonProperty("duration")]
        public Distance Duration { get; set; }

        [JsonProperty("end_address")]
        public string EndAddress { get; set; }

        [JsonProperty("end_location")]
        public Northeast EndLocation { get; set; }

        [JsonProperty("start_address")]
        public string StartAddress { get; set; }

        [JsonProperty("start_location")]
        public Northeast StartLocation { get; set; }

        [JsonProperty("steps")]
        public Step[] Steps { get; set; }

        [JsonProperty("traffic_speed_entry")]
        public object[] TrafficSpeedEntry { get; set; }

        [JsonProperty("via_waypoint")]
        public object[] ViaWaypoint { get; set; }
    }

    public partial class Distance {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }

    public partial class Step {
        [JsonProperty("distance")]
        public Distance Distance { get; set; }

        [JsonProperty("duration")]
        public Distance Duration { get; set; }

        [JsonProperty("end_location")]
        public Northeast EndLocation { get; set; }

        [JsonProperty("html_instructions")]
        public string HtmlInstructions { get; set; }

        [JsonProperty("polyline")]
        public Polyline Polyline { get; set; }

        [JsonProperty("start_location")]
        public Northeast StartLocation { get; set; }

        [JsonProperty("travel_mode")]
        public string TravelMode { get; set; }
    }

    public partial class Polyline {
        [JsonProperty("points")]
        public string Points { get; set; }
    }
}
