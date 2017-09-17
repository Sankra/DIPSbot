using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class TrondheimMessageHandler : IMessageHandler
    {
        readonly ISlackIntegration slackIntegration;

        public TrondheimMessageHandler(ISlackIntegration slackIntegration)
        {
            this.slackIntegration = slackIntegration;
        }

        public async Task HandleMessage(SlackMessage message)
        {
            if (message.MentionsBot == true) {
                var normalizedText = message.Text.ToLower();
                if (normalizedText.Contains("kjøkken")) {
                    await slackIntegration.SendMessageToChannel(message.ChatHub, "Alle andre enn Runar.");    
                }
            }
        }
    }
}
