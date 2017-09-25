using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
	struct ThanksPredicate : IPredicate
	{
		public string CommandText => "du er min yndlingsbot ❤️";

		public bool ShouldExecute(SlackMessage message) =>
			message.Text.Contains("thank you") ||
			message.Text.Contains("thanks") ||
			message.Text.Contains("yndlingsbot") ||
			message.Text.Contains("takk");
	}
}
