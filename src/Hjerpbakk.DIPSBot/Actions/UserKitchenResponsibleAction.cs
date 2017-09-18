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

		public string CommandText => "kjøkken";

		public bool ShouldExecute(SlackMessage message) =>
			message.Text.Contains("kjøkken");
	}
}

