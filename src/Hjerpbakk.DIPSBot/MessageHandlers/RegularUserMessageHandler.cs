using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class RegularUserMessageHandler : IMessageHandler
    {
		readonly ISlackIntegration slackIntegration;

		public RegularUserMessageHandler(ISlackIntegration slackIntegration)
		{
			this.slackIntegration = slackIntegration;
		}

        public async Task HandleMessage(SlackMessage message)
        {
            
        }
    }
}
