using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class DefaultMessageHandler : IMessageHandler
    {
		readonly ISlackIntegration slackIntegration;

		public DefaultMessageHandler(ISlackIntegration slackIntegration)
		{
			this.slackIntegration = slackIntegration;
		}

        public async Task HandleMessage(SlackMessage message)
        {
            
        }
    }
}
