using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlackConnector.Models;
using Hjerpbakk.DIPSbot;

namespace Hjerpbakk.DIPSBot.Actions
{
    class ListCommandsAction : IAction
    {
        readonly ISlackIntegration slackIntegration;
        readonly IEnumerable<IAction> availableActions;

        public ListCommandsAction(ISlackIntegration slackIntegration, IEnumerable<IAction> availableActions)
        {
            this.slackIntegration = slackIntegration;
            this.availableActions = availableActions;
        }

        public string CommandText => "";

        public async Task Execute(SlackMessage message)
        {
            var actions = string.Join("", availableActions.Where(a => !string.IsNullOrEmpty(a.CommandText)).Select(a => "- " + a.CommandText + "\n"));
            var availableCommands = "*Available commands*\n" + actions;
            await slackIntegration.SendDirectMessage(message.User, availableCommands);
        }

        public bool ShouldExecute(SlackMessage message) => true;
    }
}
