using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
    interface IPredicate
    {
		string CommandText { get; }

		bool ShouldExecute(SlackMessage message);
    }
}
