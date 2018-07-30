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
            // TODO: Gjør det mulig å få ut veien fra der du er, til holdeplassen, via sykling til dropoff, til dit skal
            var actionAndAddress = GetUserAddressFromMessage();
            var userAddress = actionAndAddress.address;
            if (string.IsNullOrEmpty(userAddress)) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, $"Cannot find near bike stations to an empty address.");
                return;
            }

            await slackIntegration.SendMessageToChannel(message.ChatHub, $"I'll find the bike stations nearest to {userAddress}...");

            try {
                var allBikeSharingStations = await bikeShareClient.GetAllBikeSharingStations(actionAndAddress.action);
                var nearestStations = await googleMapsClient.FindBikeSharingStationsNearestToAddress(userAddress, allBikeSharingStations);
                var response = FormatResponse(nearestStations);
                await slackIntegration.SendMessageToChannel(message.ChatHub, response.response);

                var directionsImage = await googleMapsClient.CreateImageWithDirections(userAddress, response.labelledBikeShareStations);
                var publicImageUrl = await imgurClient.UploadImage(directionsImage);
                var directionsImageAttachment = new SlackAttachment { ImageUrl = publicImageUrl };
                await slackIntegration.SendMessageToChannel(message.ChatHub,
                                                            "Here's how you get there:",
                                                            directionsImageAttachment);
            } catch (Exception e) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, $"Could not route to any bike station: {e.Message}");
                return;
            }

            (Intention action, string address) GetUserAddressFromMessage() {
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
                var addressIndex = message.Text.IndexOf(cleanedMessageText, StringComparison.CurrentCulture);
                var address = originalMessage.Substring(addressIndex, cleanedMessageText.Length);

                var modifier = originalMessage.Substring(0, originalMessage.Length - addressIndex);
                var action = Intention.Either;
                if (modifier.Contains("pick-up") || modifier.Contains("pickup") || modifier.Contains("hent")) {
                    action = Intention.PickUp;
                } else if (modifier.Contains("drop-off") || modifier.Contains("dropopp") || modifier.Contains("lever")) {
                    action = Intention.DropOff;
                }

                return (action, address);
            }

            (string response, LabelledBikeShareStation[] labelledBikeShareStations) FormatResponse(BikeShareStationWithWalkingDuration[] nearestStations) {
                var labelledBikeShareStations = new LabelledBikeShareStation[nearestStations.Length];
                var response = string.Empty;
                for (int i = 0; i < nearestStations.Length; i++) {
                    var nearStation = nearestStations[i].BikeShareStation;
                    var label = (char)('A' + i);
                    labelledBikeShareStations[i] = new LabelledBikeShareStation(label, nearStation);

                    var walkingDuration = nearestStations[i].WalkingDuration;
                    var timeToWalkToStation = walkingDuration < 86400L ? TimeSpan.FromSeconds(walkingDuration).ToString(@"hh\:mm\:ss") : "too long";
                    response += "\n" + $"{nearStation.Name} ({label}), {nearStation.Address}, {nearStation.FreeBikes} free bikes / {nearStation.AvailableSpace} free locks. Estimated walking time from {userAddress} is {timeToWalkToStation}.";
                }

                return (response, labelledBikeShareStations);
            }
        }
    }
}
