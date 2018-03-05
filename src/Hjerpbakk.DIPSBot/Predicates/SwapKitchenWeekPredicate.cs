using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
    struct SwapKitchenWeekPredicate : IPredicate
    {
        public string CommandText => "bytte 3BF";

        public bool ShouldExecute(SlackMessage message) =>
            message.Text.Contains("bytte") && message.Text.Contains("<@");
    }
}
