using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.MessageHandlers;
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
            // TODO: Returnere 2 andre alternativer også og ta de med på bildet
            // TODO: Gjør det mulig å få ut veien fra der du er, til holdeplassen, via sykling til dropoff, til dit skal
            var userAddress = GetUserAddressFromMessage();
            if (string.IsNullOrEmpty(userAddress)) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, $"Cannot find nearest bike station to an empty address.");
                return;
            }

            await slackIntegration.SendMessageToChannel(message.ChatHub, $"I'll find the bike station nearest to {userAddress}...");

            try {
                var allBikeSharingStations = await bikeShareClient.GetAllBikeSharingStations();
                var nearestStation = await googleMapsClient.FindBikeSharingStationNearestToAddress(userAddress, allBikeSharingStations);
                var directionsImage = await googleMapsClient.CreateImageWithDirections(userAddress, nearestStation);
                var publicImageUrl = await imgurClient.UploadImage(directionsImage);
                var directionsImageAttachment = new SlackAttachment { ImageUrl = publicImageUrl };
                var response = $"{nearestStation.Name}, {nearestStation.Address}, {nearestStation.FreeBikes} free bikes / {nearestStation.AvailableSpace} free locks. Estimated walking time from {userAddress} is {TimeSpan.FromSeconds(nearestStation.Distance).ToString(@"hh\:mm\:ss")}.";
                await slackIntegration.SendMessageToChannel(message.ChatHub,
                                                            response,
                                                            directionsImageAttachment);
            } catch (Exception e) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, $"Could not route to a bike station: {e.Message}");
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
