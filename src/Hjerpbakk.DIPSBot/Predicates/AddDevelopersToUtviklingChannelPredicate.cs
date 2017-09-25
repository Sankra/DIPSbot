using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
    struct AddDevelopersToUtviklingChannelPredicate : IPredicate
    {
        public string CommandText => "utv";

        public bool ShouldExecute(SlackMessage message) => message.Text == "utv";
    }
}
