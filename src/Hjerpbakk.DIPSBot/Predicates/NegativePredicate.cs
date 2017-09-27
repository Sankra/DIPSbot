using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
	struct NegativePredicate : IPredicate
	{
		public string CommandText => "";

		public bool ShouldExecute(SlackMessage message) =>
			message.Text.Contains("smart-ass") ||
			message.Text.Contains("fuck you") ||
            message.Text.Contains("slem") ||
            message.Text.Contains("stille") ||
            message.Text.Contains("dust") ||
			message.Text.Contains("damn");
	}
}
