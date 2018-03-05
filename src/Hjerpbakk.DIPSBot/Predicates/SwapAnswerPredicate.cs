using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
    struct SwapAnswerPredicate : IPredicate
    {
        public string CommandText => "";

        public bool ShouldExecute(SlackMessage message) =>
            message.Text == "ja" || message.Text == "nei";
    }
}
