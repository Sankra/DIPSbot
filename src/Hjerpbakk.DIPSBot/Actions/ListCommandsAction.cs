using System.Threading.Tasks;
using SlackConnector.Models;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.MessageHandlers;

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

        public async Task Execute(SlackMessage message, MessageHandler caller)
        {
            var availableCommands = "*Available commands*\n" + availableActions;
            await slackIntegration.SendDirectMessage(message.User, availableCommands);
        }
    }
}
