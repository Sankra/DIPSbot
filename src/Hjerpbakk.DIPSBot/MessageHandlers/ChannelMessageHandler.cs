using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class ChannelMessageHandler : MessageHandler
    {
        readonly ISlackIntegration slackIntegration;

        public ChannelMessageHandler(ISlackIntegration slackIntegration)
        {
            this.slackIntegration = slackIntegration;
        }

        public async Task HandleMessage(SlackMessage message)
        {
            
        }
    }
}
