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

        // TODO: support calculating the cost of biking for more than 45 mins and present that to the user
        // TODO: also, let the user use cheapskate mode: no costs can be allowed
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

        // TODO: Bug i boten
        // TODO: Kan ta sentrum av Trondheim eller Bergen som default, noe annet dersom bot blir spurt direkte
        //        Runar Ovesen Hjerpbakk[10:21]
        //          @DIPS-bot sykkel

        //          DIPS-bot APP[10:21]
        //          I'll find the bike stations nearest to ...
        //          Routing to bike sharing station failed: Could not find any walking routes from to 60.3916807123137,5.32789500356228|60.41292872156,5.32047362736728|60.4085821027847,5.32264858314181|60.3796857654707,5.35199363048093|60.3926768352763,5.31730828518914|60.3972375852603,5.31400131150076|60.3885300248931,5.31866523146437|60.3896720235288,5.33360508520195|60.3841809271021,5.33316077593804|60.3949312341227,5.31531945744109|60.3822547455628,5.33233246991085|60.3926514633774,5.32897691198161|60.3930981224131,5.32702272492588|60.3903244307599,5.33243965997826|60.3924553616501,5.32018517609686|60.3780246415187,5.35081815721242|60.3919836004134,5.32661819461282|60.3854312433031,5.33743178006262|60.3923426995371,5.31489050385062|60.3736379206283,5.35659456239955|60.3691798449766,5.34944379316585|60.3871980179501,5.32298004623954|60.3881668536764,5.32833480821864|60.382595986002,5.3253323435456|60.3914548486561,5.31889235970084|60.3895425243777,5.32962226854579|60.3998765227295,5.30468630789073|60.377119520251,5.33572172855838|60.395877002094,5.3077558277073|60.3937553200247,5.32179209580136.
    }
}
