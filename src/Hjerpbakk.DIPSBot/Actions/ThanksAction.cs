using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.MessageHandlers;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class ThanksAction : IAction
    {
		readonly ISlackIntegration slackIntegration;
		
		public ThanksAction(ISlackIntegration slackIntegration)
		{
			this.slackIntegration = slackIntegration;
		}

        public async Task Execute(SlackMessage message, MessageHandler caller) =>
            await slackIntegration.SendMessageToChannel(message.ChatHub, "You're welcome \ud83d\ude03");
    }
}
