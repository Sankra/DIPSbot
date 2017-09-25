using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
    struct KitchenOverviewPredicate : IPredicate
    {
		public string CommandText => "kjøkken";

		public bool ShouldExecute(SlackMessage message) =>
			message.Text.Contains("kjøkken");
    }
}
