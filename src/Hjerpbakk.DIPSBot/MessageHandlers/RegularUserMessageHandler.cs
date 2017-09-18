using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Actions;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class RegularUserMessageHandler : MessageHandler
    {
		readonly ISlackIntegration slackIntegration;
        readonly UserKitchenResponsibleAction kitchenResponsibleActions;

		public RegularUserMessageHandler(ISlackIntegration slackIntegration, UserKitchenResponsibleAction kitchenResponsibleActions)
		{
			this.slackIntegration = slackIntegration;
			this.kitchenResponsibleActions = kitchenResponsibleActions;

            // TODO: Legg inn unknown command action til slutt som lister alle commands
            actions.Add(kitchenResponsibleActions);
		}
    }
}
