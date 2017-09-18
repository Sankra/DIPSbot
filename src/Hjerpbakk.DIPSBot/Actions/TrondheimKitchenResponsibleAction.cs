using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class TrondheimKitchenResponsibleAction : KitchenResponsibleAction, IAction
    {
		public TrondheimKitchenResponsibleAction(ISlackIntegration slackIntegration, 
                                        IKitchenResponsibleClient kitchenResponsibleClient) : base(
                                            slackIntegration,
                                            kitchenResponsibleClient)
		{
		}

		public string CommandText => "";

		public bool ShouldExecute(SlackMessage message) =>
			message.MentionsBot == true && message.Text.Contains("kjøkken");
    }
}
