using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
    struct ComicsPredicate : IPredicate
    {
		public string CommandText => "comic";

		public bool ShouldExecute(SlackMessage message) =>
			message.Text.Contains("comic") || message.Text.Contains("tegneserie");
    }
}
