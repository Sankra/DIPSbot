using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Actions;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class RegularUserMessageHandler : IMessageHandler
    {
		readonly ISlackIntegration slackIntegration;
		readonly KitchenResponsibleActions kitchenResponsibleActions;

		public RegularUserMessageHandler(ISlackIntegration slackIntegration, KitchenResponsibleActions kitchenResponsibleActions)
		{
			this.slackIntegration = slackIntegration;
			this.kitchenResponsibleActions = kitchenResponsibleActions;
		}

        public async Task HandleMessage(SlackMessage message)
        {
			if (message.Text.Contains("kjøkken"))
			{
				await kitchenResponsibleActions.SendMessageWithKitchenResponsibles(message);
            } else {
                await slackIntegration.SendDirectMessage(message.User, "Unknown command");
            }
        }
    }
}
