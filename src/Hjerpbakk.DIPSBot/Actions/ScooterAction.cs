using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.MessageHandlers;
using Hjerpbakk.DIPSBot.Model;
using Newtonsoft.Json;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class ScooterAction : IAction
    {
        readonly ISlackIntegration slackIntegration;
        readonly HttpClient httpClient;
        readonly IList<ScooterQuote> scooterQuotes;

        readonly Random random;

        public ScooterAction(ISlackIntegration slackIntegration, HttpClient httpClient)
        {
            this.slackIntegration = slackIntegration;
            this.httpClient = httpClient;

            random = new Random();
            scooterQuotes = JsonConvert.DeserializeObject<IList<ScooterQuote>>(File.ReadAllText("scooter.json"));
        }

        public async Task Execute(SlackMessage message, MessageHandler caller)
        {
            var i = random.Next(0, scooterQuotes.Count);
            var attachment = new SlackAttachment
            {
                Text = scooterQuotes[i].Lyric,
                AuthorName = scooterQuotes[i].Song
            };

            await slackIntegration.SendMessageToChannel(message.ChatHub, "Scooter says:", attachment);
        }
    }
}
