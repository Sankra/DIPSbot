using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.MessageHandlers;
using Hjerpbakk.DIPSBot.Model.BikeShare;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions {
    class BikeShareAction : IAction {
        readonly ISlackIntegration slackIntegration;
        readonly BikeShareClient bikeShareClient;
        readonly GoogleMapsClient googleMapsClient;
        readonly ImgurClient imgurClient;

        public BikeShareAction(ISlackIntegration slackIntegration, BikeShareClient bikeShareClient, GoogleMapsClient googleMapsClient, ImgurClient imgurClient) {
            this.slackIntegration = slackIntegration;
            this.bikeShareClient = bikeShareClient;
            this.googleMapsClient = googleMapsClient;
            this.imgurClient = imgurClient;
        }

        public async Task Execute(SlackMessage message, MessageHandler caller) {
            // TODO: Finne nærmeste holdeplass med ledig plass til å legge fra seg sykkel
            // TODO: Finne nærmes holdeplass for å hente seg sykkel
            // TODO: Gjør det mulig å få ut veien fra der du er, til holdeplassen, via sykling til dropoff, til dit skal
            var userAddress = GetUserAddressFromMessage();
            if (string.IsNullOrEmpty(userAddress)) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, $"Cannot find near bike stations to an empty address.");
                return;
            }

            await slackIntegration.SendMessageToChannel(message.ChatHub, $"I'll find the bike stations nearest to {userAddress}...");

            try {
                var allBikeSharingStations = await bikeShareClient.GetAllBikeSharingStations();
                var nearestStations = await googleMapsClient.FindBikeSharingStationsNearestToAddress(userAddress, allBikeSharingStations);
                var labelledBikeShareStations = new LabelledBikeShareStation[nearestStations.Length];
                var response = string.Empty;
                for (int i = 0; i < nearestStations.Length; i++) {
                    var neartStation = nearestStations[i];
                    var label = (char)('A' + i);
                    labelledBikeShareStations[i] = new LabelledBikeShareStation(label, nearestStations[i]);
                    var timeToWalkToStation = neartStation.WalkingDuration < 86400L ? TimeSpan.FromSeconds(neartStation.WalkingDuration).ToString(@"hh\:mm\:ss") : "too long";
                    response += "\n" + $"{neartStation.Name} ({label}), {neartStation.Address}, {neartStation.FreeBikes} free bikes / {neartStation.AvailableSpace} free locks. Estimated walking time from {userAddress} is {timeToWalkToStation}.";
                }

                await slackIntegration.SendMessageToChannel(message.ChatHub, response);

                var directionsImage = await googleMapsClient.CreateImageWithDirections(userAddress, labelledBikeShareStations);
                var publicImageUrl = await imgurClient.UploadImage(directionsImage);
                var directionsImageAttachment = new SlackAttachment { ImageUrl = publicImageUrl };
                await slackIntegration.SendMessageToChannel(message.ChatHub,
                                                            "Here's how you get there",
                                                            directionsImageAttachment);
            } catch (Exception e) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, $"Could not route to any bike station: {e.Message}");
                return;
            }

            string GetUserAddressFromMessage() {
                var cleanedMessageText = message.Text;
                while (cleanedMessageText.IndexOf('<') != -1) {
                    var i = cleanedMessageText.IndexOf('<');
                    var j = cleanedMessageText.IndexOf('>');
                    if (j == -1) {
                        break;
                    }

                    cleanedMessageText = cleanedMessageText.Remove(i, j - i + 1);
                }

                const string Bike = "bike";
                const string Sykkel = "sykkel";
                var bikeIndex = cleanedMessageText.IndexOf(Bike, StringComparison.CurrentCulture);
                bikeIndex = bikeIndex == -1 ? cleanedMessageText.IndexOf(Sykkel, StringComparison.CurrentCulture) : bikeIndex;
                cleanedMessageText = cleanedMessageText.Remove(0, bikeIndex).Replace(Bike, "").Replace(Sykkel, "").Trim();

                var jsonObject = (JObject)JsonConvert.DeserializeObject(message.RawData);
                var originalMessage = (string)jsonObject.Property("text").Value;
                return originalMessage.Substring(message.Text.IndexOf(cleanedMessageText, StringComparison.CurrentCulture), cleanedMessageText.Length);
            }
        }
    }
}
