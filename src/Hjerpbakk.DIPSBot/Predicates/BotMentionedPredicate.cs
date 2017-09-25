using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
	struct BotMentionedPredicate : IPredicate
	{
		public string CommandText => "";
        public bool ShouldExecute(SlackMessage message) => message.MentionsBot;
	}
}