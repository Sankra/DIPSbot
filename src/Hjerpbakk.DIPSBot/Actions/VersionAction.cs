using System.Reflection;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    public class VersionAction : IAction
    {
        readonly ISlackIntegration slackIntegration;

        public VersionAction(ISlackIntegration slackIntegration)
        {
            this.slackIntegration = slackIntegration;
        }

        public async Task Execute(SlackMessage message) =>
            await slackIntegration.SendMessageToChannel(message.ChatHub, 
                                                        Assembly.GetExecutingAssembly().GetName().Version.ToString());
    }
}
