using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.MessageHandlers;
using Hjerpbakk.DIPSBot.Model.BikeSharing;
using Hjerpbakk.DIPSBot.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions {
    class BikeSharingAction : IAction {
        readonly ISlackIntegration slackIntegration;
        readonly BikeSharingService bikeSharingService;

        public BikeSharingAction(ISlackIntegration slackIntegration, BikeSharingService bikeSharingService) {
            this.slackIntegration = slackIntegration;
            this.bikeSharingService = bikeSharingService;
        }

        public async Task Execute(SlackMessage message, MessageHandler caller) {
            var locationAndIntention = GetUserLocationAndIntentFromMessage();
            if (string.IsNullOrEmpty(locationAndIntention.location)) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, $"Cannot find near bike stations to an empty address.");
                return;
            }

            await slackIntegration.SendMessageToChannel(message.ChatHub, $"I'll find the bike stations nearest to {locationAndIntention.location}...");

            try {
                var bikeSharingStationsInformation = await bikeSharingService.GetInformationOnNearestBikeSharingStations(locationAndIntention.location, locationAndIntention.intention);
                foreach (var response in bikeSharingStationsInformation.Response) {
                    await slackIntegration.SendMessageToChannel(message.ChatHub, response);
                }

                var publicImageUrl = await bikeSharingService.GetImageWithDirections(locationAndIntention.location, bikeSharingStationsInformation.LabelledBikeSharingStations);
                var directionsImageAttachment = new SlackAttachment { ImageUrl = publicImageUrl };
                await slackIntegration.SendMessageToChannel(message.ChatHub,
                                                            "Here's how you get there:",
                                                            directionsImageAttachment);
            } catch (Exception e) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, $"Routing to bike sharing station failed: {e.Message}");
                return;
            }

            (Intention intention, string location) GetUserLocationAndIntentFromMessage() {
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
        }
    }
}
