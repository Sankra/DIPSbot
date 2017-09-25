using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
    struct TruePredicate : IPredicate
    {
        public string CommandText => "";
        public bool ShouldExecute(SlackMessage message) => true;
    }
}
