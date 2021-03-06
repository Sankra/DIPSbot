﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Hjerpbakk.DIPSbot;
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
            try {
                var (intention, location, destination) = GetUserLocationAndIntentFromMessage();

                switch (intention) {
                    case Intention.QuickestRoute:
                        await GetQuickestRoute(location, destination);
                        break;
                    default:
                        await FindNearestBikeSharingStation(intention, location);
                        break;
                }
            } catch (Exception e) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, $"Routing to bike sharing station failed: {e.Message}");
            }

            async Task GetQuickestRoute(Address from, Address to) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, $"Finding quickest route from {from} to {to}...");
                var directionsAndImage = await bikeSharingService.GetQuickestRoute(from, to);
                var directionsImageAttachment = new SlackAttachment { ImageUrl = directionsAndImage.ImageUrl };
                await slackIntegration.SendMessageToChannel(message.ChatHub,
                                                            directionsAndImage.Directions,
                                                            directionsImageAttachment);
            }

            (Intention intention, Address location, Address destination) GetUserLocationAndIntentFromMessage() {
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
                var originalMessage = HttpUtility.HtmlDecode((string)jsonObject.Property("text").Value);
                var addressIndex = message.Text.IndexOf(cleanedMessageText, StringComparison.CurrentCulture);
                var address = originalMessage.Substring(addressIndex, cleanedMessageText.Length);

                var (index, length) = (address.IndexOf("->", StringComparison.CurrentCulture), 2);
                if (index == -1) {
                    (index, length) = (address.IndexOf(">", StringComparison.CurrentCulture), 1);
                }

                if (index == -1) {
                    var modifier = originalMessage.Substring(0, originalMessage.Length - addressIndex);
                    var intention = Intention.Either;
                    if (modifier.Contains("pick-up") || modifier.Contains("pickup") || modifier.Contains("hent")) {
                        intention = Intention.PickUp;
                    } else if (modifier.Contains("drop-off") || modifier.Contains("dropopp") || modifier.Contains("deliver") || modifier.Contains("lever")) {
                        intention = Intention.DropOff;
                    }

                    return (intention, new Address(address), new Address());
                }

                var from = address.Substring(0, index - 1);
                var to = address.Substring(index + length + 1, address.Length - index - length - 1);
                return (Intention.QuickestRoute, new Address(from), new Address(to));
            }

            async Task FindNearestBikeSharingStation(Intention intention, Address location) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, $"I'll find the bike stations nearest to {location}...");

                var bikeSharingStationsInformation = await bikeSharingService.GetInformationOnNearestBikeSharingStations(location, intention);
                foreach (var response in bikeSharingStationsInformation.Response) {
                    await slackIntegration.SendMessageToChannel(message.ChatHub, response);
                }

                var publicImageUrl = await bikeSharingService.GetImageWithDirections(location, bikeSharingStationsInformation.LabelledBikeSharingStations);
                var directionsImageAttachment = new SlackAttachment { ImageUrl = publicImageUrl };
                await slackIntegration.SendMessageToChannel(message.ChatHub,
                                                            "Here's how you get there:",
                                                            directionsImageAttachment);
            }
        }
    }
}
