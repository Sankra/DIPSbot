using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.MessageHandlers;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class NegativeAction : IAction
    {
		readonly ISlackIntegration slackIntegration;
		
		public NegativeAction(ISlackIntegration slackIntegration)
		{
			this.slackIntegration = slackIntegration;
		}

        public async Task Execute(SlackMessage message, MessageHandler caller) =>
            await slackIntegration.SendMessageToChannel(message.ChatHub, "Hadde jeg hatt følelser, ville jeg følt \ud83d\ude2d akkurat nå \ud83d\ude33");
    }
}
