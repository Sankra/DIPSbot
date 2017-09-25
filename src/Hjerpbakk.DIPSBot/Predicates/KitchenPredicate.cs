using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
    struct KitchenPredicate : IPredicate
    {
		public string CommandText => "kjøkken";

		public bool ShouldExecute(SlackMessage message) =>
			message.Text.Contains("kjøkken");
    }
}
