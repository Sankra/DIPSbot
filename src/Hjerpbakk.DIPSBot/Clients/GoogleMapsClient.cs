using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using BikeshareClient.Models;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Model.BikeShare;
using Microsoft.Extensions.Caching.Memory;
using Hjerpbakk.DIPSBot.Extensions;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace Hjerpbakk.DIPSBot.Clients {
    public class GoogleMapsClient {
        readonly HttpClient httpClient;

        readonly string baseDistanceQueryString;
        readonly string baseRouteQueryString;
        readonly string baseImageUrl;

        readonly IMemoryCache memoryCache;

        public GoogleMapsClient(IGoogleMapsConfiguration googleMapsConfiguration, HttpClient httpClient, IMemoryCache memoryCache) {
            this.httpClient = httpClient;
            baseDistanceQueryString = "https://maps.googleapis.com/maps/api/distancematrix/json?origins={0}&destinations={1}&region=no&mode=walking&units=metric&key=" + googleMapsConfiguration.GoogleMapsApiKey;
            baseRouteQueryString = "https://maps.googleapis.com/maps/api/directions/json?origin={0}&destination={1}&region=no&mode=walking&units=metric&key=" + googleMapsConfiguration.GoogleMapsApiKey;
            // TODO: Decide for or against custom icon
            // TODO: Decide size
            baseImageUrl = "https://maps.googleapis.com/maps/api/staticmap?size=600x600&scale=2&maptype=roadmap&region=no&markers=icon:https://hjerpbakk.com/assets/img/person.png%7C{0}&markers=icon:https://hjerpbakk.com/assets/img/parking.png%3F1%7C{1}&path=weight:5%7Ccolor:blue%7Cenc:{2}&key=" + googleMapsConfiguration.GoogleMapsApiKey;
            this.memoryCache = memoryCache;
        }

        public async Task<BikeStation> FindBikeSharingStationNearestToAddress(string fromAddress, AllStationsInArea allStationsInArea) {
            if (string.IsNullOrEmpty(fromAddress)) {
                throw new ArgumentNullException(nameof(fromAddress));
            }

            var encodedAddress = HttpUtility.UrlEncode(fromAddress);
            var routeDistances = await memoryCache.GetOrSet(encodedAddress, FindRoutesToAllStations);

            var sortedStations = SortStationsByDistanceFromUser();
            var nearestStation = sortedStations.First().station;
            var stationStatus = allStationsInArea.StationsStatus.Single(s => s.Id == nearestStation.Id);
            return new BikeStation(nearestStation.Name,
                               nearestStation.Address,
                               stationStatus.BikesAvailable,
                               stationStatus.DocksAvailable,
                               nearestStation.Latitude,
                               nearestStation.Longitude,
                               sortedStations.First().distance);

            async Task<Element[]> FindRoutesToAllStations() {
                var queryString = string.Format(baseDistanceQueryString, encodedAddress, allStationsInArea.PipedCoordinatesToAllStations);
                var response = await httpClient.GetStringAsync(queryString);
                var routeDistance = JsonConvert.DeserializeObject<RouteDistance>(response);

                if (routeDistance.Rows.Length == 0) {
                    throw new InvalidOperationException($"Could not find any routes from {fromAddress} to any bike sharing stations.");
                }

                return routeDistance.Rows[0].Elements;
            }

            (long distance, Station station)[] SortStationsByDistanceFromUser() {
                var reachableStations = new List<(long distance, Station station)>();
                for (int i = 0; i < routeDistances.Length; i++) {
                    var element = routeDistances[i];
                    if (element.Status != "OK") {
                        continue;
                    }

                    reachableStations.Add((element.Duration.Value, allStationsInArea.Stations[i]));
                }

                var nearestStations = reachableStations.OrderBy(s => s.distance).ToArray();
                return nearestStations;
            }
        }

        public async Task<string> CreateImageWithDirections(string from, BikeStation bikeStation) {
            var routePolyline = await memoryCache.GetOrSet(from + "directions", FindDetailedRouteToStation);
            var imageUrl = string.Format(baseImageUrl, from, $"{bikeStation.Latitude},{bikeStation.Longitude}", routePolyline);

            return imageUrl;

            async Task<string> FindDetailedRouteToStation() {
                var queryString = string.Format(baseRouteQueryString, from, $"{bikeStation.Latitude},{bikeStation.Longitude}");
                var response = await httpClient.GetStringAsync(queryString);
                var route = JsonConvert.DeserializeObject<Route>(response);
                if (route.Status != "OK" || route.Routes.Length == 0) {
                    throw new InvalidOperationException($"Could not find a route from {from} to {bikeStation.Name}, {bikeStation.Address}.");
                }

                return route.Routes[0].OverviewPolyline.Points;
            }
        }
    }
}
