using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Extensions;
using Hjerpbakk.DIPSBot.Model.BikeSharing;
using Hjerpbakk.DIPSBot.Model.BikeSharing.Google;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Hjerpbakk.DIPSBot.Clients {
    public class GoogleMapsClient {
        const int MaxResultSize = 3;

        readonly string baseDistanceQueryString;
        readonly string baseRouteQueryString;
        readonly string baseImageUrl;

        readonly HttpClient httpClient;
        readonly IMemoryCache memoryCache;

        public GoogleMapsClient(IGoogleMapsConfiguration googleMapsConfiguration, HttpClient httpClient, IMemoryCache memoryCache) {
            this.httpClient = httpClient;

            var region = string.IsNullOrEmpty(googleMapsConfiguration.MapsRegion) ? string.Empty : "&region=" + googleMapsConfiguration.MapsRegion;
            // Google Maps Distance Matrix API
            baseDistanceQueryString = "https://maps.googleapis.com/maps/api/distancematrix/json?origins={0}&destinations={1}" + region + "&mode={2}&units=metric&key=" + googleMapsConfiguration.GoogleMapsApiKey;
            // Google Maps Directions API
            baseRouteQueryString = "https://maps.googleapis.com/maps/api/directions/json?origin={0}&destination={1}" + region + "&mode={2}&units=metric&key=" + googleMapsConfiguration.GoogleMapsApiKey;
            // Google Maps Static Image API
            baseImageUrl = "https://maps.googleapis.com/maps/api/staticmap?size=600x600&scale=2&maptype=roadmap" + region + "&{0}&{1}&{2}&key=" + googleMapsConfiguration.GoogleMapsApiKey;
            this.memoryCache = memoryCache;
        }

        public async Task<BikeSharingStationWithWalkingDuration[]> FindBikeSharingStationsNearestToLocation(Address location, AllStationsInArea allStationsInArea) {
            var route = new Route(location, allStationsInArea.PipedCoordinatesToAllStations, TransportationMode.Walking);
            var routeDistances = await FindDurationToDestinations(route);
            var sortedStations = SortStationsByDistanceFromUser();
            var resultLength = sortedStations.Length < MaxResultSize ? sortedStations.Length : MaxResultSize;
            var nearestStations = new BikeSharingStationWithWalkingDuration[resultLength];
            for (int i = 0; i < nearestStations.Length; i++) {
                nearestStations[i] = new BikeSharingStationWithWalkingDuration(
                    allStationsInArea.BikeSharingStations[sortedStations[i].index],
                    sortedStations[i].duration);
            }

            return nearestStations;

            (Duration duration, int index)[] SortStationsByDistanceFromUser() {
                var reachableStations = new List<(Duration duration, int index)>();
                for (int i = 0; i < routeDistances.Length; i++) {
                    var element = routeDistances[i];
                    if (element.Status != "OK") {
                        continue;
                    }

                    reachableStations.Add((element.Duration, i));
                }

                return reachableStations.OrderBy(s => s.duration.Value).ToArray(); ;
            }
        }

        public async Task<string> CreateImageWithDirections(Address from, LabelledBikeSharingStation[] nearestBikeStations) {
            if (nearestBikeStations == null || nearestBikeStations.Length == 0 || nearestBikeStations.Length > MaxResultSize) {
                throw new ArgumentException($"Number of bike sharing stations must be between 1 and {MaxResultSize}", nameof(nearestBikeStations));
            }

            var routePolyline = await memoryCache.GetOrSet(from.Value + "directions", FindDetailedRouteToStation);
            var userMarker = $"markers=color:green%7Clabel:U%7C{from}";
            var stationMarkers = string.Join("&", nearestBikeStations.Select(bikeSharingStation => $"markers=color:red%7Clabel:{bikeSharingStation.Label}%7C{bikeSharingStation.BikeSharingStation.Address.Value}"));
            var imageUrl = string.Format(baseImageUrl, userMarker, stationMarkers, "path=weight:5%7Ccolor:blue%7Cenc:" + routePolyline.Points);
            return imageUrl;

            async Task<Polyline> FindDetailedRouteToStation() {
                var bikeSharingStation = nearestBikeStations[0].BikeSharingStation;
                var route = new Route(from, bikeSharingStation.Address, TransportationMode.Walking);
                return (await GetDetailedRoute(route)).Segment;
            }
        }

        // TODO: Use colors as the bysykkel in that city uses
        public string CreateRouteImage(TransportationMode transportationMode, params RouteSegment[] routeSegments) {
            if (routeSegments.Length == 0) {
                throw new ArgumentException(nameof(routeSegments));
            }

            var from = $"markers=color:green%7Clabel:U%7C{routeSegments[0].Route.From}";
            var to = $"markers=color:red%7Clabel:D%7C{routeSegments[routeSegments.Length - 1].Route.To}";
            var path = $"path=weight:5%7Ccolor:green%7Cenc:{routeSegments[0].Segment.Points}";
            switch (transportationMode) {
                case TransportationMode.Walking:
                    return string.Format(baseImageUrl, from, to, path);
                case TransportationMode.Bicycling:
                    var stations = $"markers=color:blue%7Clabel:B%7C{routeSegments[1].Route.From.UrlEncodedValue}%7C{routeSegments[1].Route.To.UrlEncodedValue}";
                    path = $"{path}&path=weight:5%7Ccolor:blue%7Cenc:{routeSegments[1].Segment.Points}&path=weight:5%7Ccolor:green%7Cenc:{routeSegments[2].Segment.Points}";
                    return string.Format(baseImageUrl, $"{from}&{to}", $"{stations}", path);
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<RouteDuration> GetDuration(Route route) {
            var routes = await FindDurationToDestinations(route);
            var validRoutes = routes.Where(r => r.Status == "OK").ToArray();
            if (validRoutes.Length == 0) {
                throw new InvalidOperationException($"Could not find any routes from {route.From} to {route.To}.");
            }

            return new RouteDuration(route, validRoutes.OrderBy(r => r.Duration).First().Duration);
        }

        public async Task<RouteSegment> GetDetailedRoute(Route route) {
            var queryString = string.Format(baseRouteQueryString, route.From.UrlEncodedValue, route.To.UrlEncodedValue, route.EncodedTransportationMode);
            var response = await httpClient.GetStringAsync(queryString);
            var googleRoute = JsonConvert.DeserializeObject<GoogleRoute>(response);
            if (googleRoute.Status != "OK" || googleRoute.Routes.Length == 0) {
                throw new InvalidOperationException($"Could not find a {route.EncodedTransportationMode} route from {route.From} to {route.To}.");
            }

            return new RouteSegment(route, googleRoute.Routes[0].OverviewPolyline);
        }

        async Task<Element[]> FindDurationToDestinations(Route route) {
            var queryString = string.Format(baseDistanceQueryString, route.From.UrlEncodedValue, route.To.UrlEncodedValue, route.EncodedTransportationMode);
            var response = await httpClient.GetStringAsync(queryString);
            var routeDistance = JsonConvert.DeserializeObject<GoogleRouteDistance>(response);

            if (routeDistance.Status != "OK" || routeDistance.Rows.Length == 0) {
                throw new InvalidOperationException($"Could not find any {route.EncodedTransportationMode} routes from {route.From} to {route.To}.");
            }

            return routeDistance.Rows[0].Elements;
        }
    }
}
