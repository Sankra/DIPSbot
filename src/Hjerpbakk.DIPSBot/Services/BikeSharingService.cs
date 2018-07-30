using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Clients;
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

        public async Task<BikeSharingStationsInformation> GetInformationOnNearestBikeSharingStations(string location, Intention intention) {
            if (string.IsNullOrEmpty(location)) {
                throw new ArgumentException($"{nameof(location)} cannot be null or empty.", nameof(location));
            }

            var bikeSharingStations = await bikeSharingClient.GetBikeSharingStationsMatchingIntention(intention);
            var nearestStations = await googleMapsClient.FindBikeSharingStationsNearestToAddress(location, bikeSharingStations);

            var labelledBikeSharingStations = new LabelledBikeSharingStation[nearestStations.Length];
            var responses = new string[nearestStations.Length];
            for (int i = 0; i < nearestStations.Length; i++) {
                var nearStation = nearestStations[i].BikeSharingStation;
                var label = (char)('A' + i);
                labelledBikeSharingStations[i] = new LabelledBikeSharingStation(label, nearStation);

                var walkingDuration = nearestStations[i].WalkingDuration;
                var timeToWalkToStation = walkingDuration < 86400L ? TimeSpan.FromSeconds(walkingDuration).ToString(@"hh\:mm\:ss") : "too long";
                responses[i] = "\n" + $"{nearStation.Name} ({label}), {nearStation.Address}, {nearStation.FreeBikes} free bikes / {nearStation.AvailableSpace} free locks. Estimated walking time from {location} is {timeToWalkToStation}.";
            }

            return new BikeSharingStationsInformation(responses, labelledBikeSharingStations);
        }

        public async Task<string> GetImageWithDirections(string location, LabelledBikeSharingStation[] labelledBikeSharingStations) {
            var directionsImage = await googleMapsClient.CreateImageWithDirections(location, labelledBikeSharingStations);
            var publicImageUrl = await imgurClient.UploadImage(directionsImage);
            return publicImageUrl;
        }

        public async Task<string> GetQuickestRoute(string from, string to) {
            if (string.IsNullOrEmpty(from)) {
                throw new ArgumentException($"{nameof(from)} cannot be null or empty.", nameof(from));
            }

            if (string.IsNullOrEmpty(to)) {
                throw new ArgumentException($"{nameof(to)} cannot be null or empty.", nameof(to));
            }

            var walkingDuration = googleMapsClient.GetWalkingDuration(from, to);

            // TODO: Combination:
            // - Walking duration to nearest free station
            // - bike duration to station nearest to destination
            // - walking duration from that station to destination
            // TODO: choose the quickest
            // TODO: show on map

            return "";
        }
    }
}
