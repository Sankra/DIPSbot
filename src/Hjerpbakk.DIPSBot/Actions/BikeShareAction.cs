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
        readonly TrondheimBysykkelClient bikeShareClient;
        readonly ImgurClient imgurClient;

        public BikeShareAction(ISlackIntegration slackIntegration, TrondheimBysykkelClient bikeShareClient, ImgurClient imgurClient) {
            this.slackIntegration = slackIntegration;
            this.bikeShareClient = bikeShareClient;
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
                var station = await bikeShareClient.FindNearesBikeSharingStation(userAddress);
                var mapsImageUrl = await bikeShareClient.FindDirectionsImage(userAddress, station);
                var imageUrl = await imgurClient.UploadImage(mapsImageUrl);
                var routeImage = new SlackAttachment { ImageUrl = imageUrl };
                var response = $"{station.Name}, {station.Address}, {station.FreeBikes} free bikes / {station.AvailableSpace} free locks. Estimated walking time from {userAddress} is {TimeSpan.FromSeconds(station.Distance).ToString(@"hh\:mm\:ss")}.";
                await slackIntegration.SendMessageToChannel(message.ChatHub,
                                                            response,
                                                            routeImage);
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
