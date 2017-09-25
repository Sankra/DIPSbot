using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    public class NegativeAction : IAction
    {
		readonly ISlackIntegration slackIntegration;
		
		public NegativeAction(ISlackIntegration slackIntegration)
		{
			this.slackIntegration = slackIntegration;
		}

        public async Task Execute(SlackMessage message) =>
            await slackIntegration.SendMessageToChannel(message.ChatHub, "Hadde jeg hatt følelser, ville jeg følt \ud83d\ude2d akkurat nå \ud83d\ude33");
    }
}
