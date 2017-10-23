using System.Threading.Tasks;
using SlackConnector.Models;
using Hjerpbakk.DIPSbot;

namespace Hjerpbakk.DIPSBot.Actions
{
    class ListCommandsAction : IAction
    {
        readonly ISlackIntegration slackIntegration;
        readonly string availableActions;

        public ListCommandsAction(ISlackIntegration slackIntegration, string availableActions)
        {
            this.slackIntegration = slackIntegration;
            this.availableActions = availableActions;
        }

        public async Task Execute(SlackMessage message)
        {
            var availableCommands = "*Available commands*\n" + availableActions;
            await slackIntegration.SendDirectMessage(message.User, availableCommands);
        }
    }
}
