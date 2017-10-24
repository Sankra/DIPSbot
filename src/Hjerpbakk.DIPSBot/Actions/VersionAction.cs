using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.MessageHandlers;
using Hjerpbakk.DIPSBot.Services;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class VersionAction : IAction
    {
        readonly ISlackIntegration slackIntegration;
        readonly IDebuggingService debuggingService;

        public VersionAction(ISlackIntegration slackIntegration, IDebuggingService debuggingService)
        {
            this.slackIntegration = slackIntegration;
            this.debuggingService = debuggingService;
        }

        public async Task Execute(SlackMessage message, MessageHandler caller) =>
            await slackIntegration.SendMessageToChannel(message.ChatHub, debuggingService.GetVersionInfo(caller));
    }
}
