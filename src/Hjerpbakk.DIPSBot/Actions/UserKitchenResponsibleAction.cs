using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
	class UserKitchenResponsibleAction : KitchenResponsibleAction, IAction
	{
		public UserKitchenResponsibleAction(ISlackIntegration slackIntegration,
										    IKitchenResponsibleClient kitchenResponsibleClient) : base(
											slackIntegration,
											kitchenResponsibleClient)
		{
		}

		public string CommandText => "";

		public bool ShouldExecute(SlackMessage message) =>
			message.Text.Contains("kjøkken");
	}
}

