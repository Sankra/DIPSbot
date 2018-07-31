using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Extensions;
using Hjerpbakk.DIPSBot.Model.BikeSharing;

namespace Hjerpbakk.DIPSBot.Services {
    public class BikeSharingService {
        readonly BikeSharingClient bikeSharingClient;
        readonly GoogleMapsClient googleMapsClient;
        readonly ImgurClient imgurClient;

        public BikeSharingService(BikeSharingClient bikeSharingClient, GoogleMapsClient googleMapsClient, ImgurClient imgurClient) {
            this.bikeSharingClient = bikeSharingClient;
            this.googleMapsClient = googleMapsClient;
            this.imgurClient = imgurClient;
        }

        public async Task<BikeSharingStationsInformation> GetInformationOnNearestBikeSharingStations(Address location, Intention intention) {
            var bikeSharingStations = await bikeSharingClient.GetBikeSharingStationsMatchingIntention(intention);
            var nearestStations = await googleMapsClient.FindBikeSharingStationsNearestToLocation(location, bikeSharingStations);

            var labelledBikeSharingStations = new LabelledBikeSharingStation[nearestStations.Length];
            var responses = new string[nearestStations.Length];
            for (int i = 0; i < nearestStations.Length; i++) {
                var nearStation = nearestStations[i].BikeSharingStation;
                var label = (char)('A' + i);
                labelledBikeSharingStations[i] = new LabelledBikeSharingStation(label, nearStation);
                responses[i] = $"{nearStation.Name} ({label}) has {nearStation.FreeBikes} free bikes / {nearStation.AvailableSpace} free locks. Estimated walking time is {nearestStations[i].WalkingDuration.Text}.";
            }

            return new BikeSharingStationsInformation(responses, labelledBikeSharingStations);
        }

        public async Task<string> GetImageWithDirections(Address location, LabelledBikeSharingStation[] labelledBikeSharingStations) {
            var directionsImage = await googleMapsClient.CreateImageWithDirections(location, labelledBikeSharingStations);
            var publicImageUrl = await imgurClient.UploadImage(directionsImage);
            return publicImageUrl;
        }

        public async Task<DirectionsAndImage> GetQuickestRoute(Address from, Address to) {
            var routeDurationTasks = await Task.WhenAll(new[] { GetWalkingRoute(), GetCombinedWalkingAndCyclingRoute() });
            var walkingRoute = routeDurationTasks[0];
            var combinedWalkingAndCyclingRoute = routeDurationTasks[1];
            var quickestRouteAndDuration = walkingRoute.duration < combinedWalkingAndCyclingRoute.duration ? walkingRoute : combinedWalkingAndCyclingRoute;

            var routeSegments = await Task.WhenAll(quickestRouteAndDuration.route.Select(async rd => await googleMapsClient.GetDetailedRoute(rd.Route)));
            var imageUrl = googleMapsClient.CreateRouteImage(quickestRouteAndDuration.transportationMode, routeSegments);
            var publicImageUrl = await imgurClient.UploadImage(imageUrl);

            var directions = quickestRouteAndDuration.transportationMode == TransportationMode.Walking
                                 ? $"🚶 You can walk directly from {from} to {to} in {quickestRouteAndDuration.route[0].Duration.Text}."
                                 : $"🚲 The quickest route from {from} to {to} will take {TimeSpan.FromSeconds(quickestRouteAndDuration.duration).FormatWithoutSeconds()}. You should {FormatRoute()}.";
            return new DirectionsAndImage(directions, publicImageUrl);

            async Task<(RouteDuration[] route, long duration, TransportationMode transportationMode)> GetWalkingRoute() {
                var route = new Route(from, to, TransportationMode.Walking);
                var routeDuration = await googleMapsClient.GetDuration(route);
                return (new[] { routeDuration }, routeDuration.Duration.Value, TransportationMode.Walking);
            }

            async Task<(RouteDuration[] route, long duration, TransportationMode transportationMode)> GetCombinedWalkingAndCyclingRoute() {
                var routeDurations = new RouteDuration[3];
                var bikeSharingStationsWithBikes = await bikeSharingClient.GetBikeSharingStationsMatchingIntention(Intention.PickUp);
                var nearestStations = await googleMapsClient.FindBikeSharingStationsNearestToLocation(from, bikeSharingStationsWithBikes);
                var startingBikeSharingStation = nearestStations.First().BikeSharingStation.Address;
                var route = new Route(from, startingBikeSharingStation, TransportationMode.Walking);
                routeDurations[0] = await googleMapsClient.GetDuration(route);

                var bikeSharingStations = await bikeSharingClient.GetBikeSharingStationsMatchingIntention(Intention.Either);
                nearestStations = await googleMapsClient.FindBikeSharingStationsNearestToLocation(to, bikeSharingStationsWithBikes);
                var endingBikeSharingStation = nearestStations.First().BikeSharingStation.Address;
                route = new Route(startingBikeSharingStation, endingBikeSharingStation, TransportationMode.Bicycling);
                routeDurations[1] = await googleMapsClient.GetDuration(route);

                route = new Route(endingBikeSharingStation, to, TransportationMode.Walking);
                routeDurations[2] = await googleMapsClient.GetDuration(route);
                var combinedDuration = routeDurations.Sum(d => d.Duration.Value) + 45;
                return (routeDurations, combinedDuration, TransportationMode.Bicycling);
            }

            string FormatRoute() {
                var routeString = new StringBuilder();
                var quickestRoute = quickestRouteAndDuration.route;
                for (int i = 0; i < quickestRoute.Length; i++) {
                    var route = quickestRoute[i].Route;
                    routeString.Append(route.TransportationMode == TransportationMode.Walking ? "walk" : "bicycle");
                    if (i == 0) {
                        routeString.Append(" from ");
                        routeString.Append(route.From);
                    }

                    routeString.Append(" to ");
                    routeString.Append(route.To);
                    routeString.Append(" (");
                    routeString.Append(quickestRoute[i].Duration.Text);
                    routeString.Append(")");
                    if (i == quickestRoute.Length - 2) {
                        routeString.Append(" and ");
                    } else if (i != quickestRoute.Length - 1) {
                        routeString.Append(", ");
                    }
                }

                return routeString.ToString();
            }
        }
    }
}
