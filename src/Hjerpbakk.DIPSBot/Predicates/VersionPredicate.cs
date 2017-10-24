using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
    struct VersionPredicate : IPredicate
	{
		public string CommandText => "version";

        public bool ShouldExecute(SlackMessage message) => message.Text.Contains(CommandText);
	}
}


