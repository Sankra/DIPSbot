using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
    struct ThanksPredicate : IPredicate
    {
		public string CommandText => "thank you";

		public bool ShouldExecute(SlackMessage message) =>
			message.Text.Contains("thank you") || message.Text.Contains("thanks");
    }
}
