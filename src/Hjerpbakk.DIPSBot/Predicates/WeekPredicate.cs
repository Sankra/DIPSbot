using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
	struct WeekPredicate : IPredicate
	{
		public string CommandText => "uke";

		public bool ShouldExecute(SlackMessage message) =>
			!message.Text.Contains("kjøkken") && (message.Text.Contains("uke") || message.Text.Contains("week"));
	}
}


