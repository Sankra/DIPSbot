using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    public class ChangeKitchenServiceIPAction : IAction
    {
		readonly ISlackIntegration slackIntegration;

		public ChangeKitchenServiceIPAction(ISlackIntegration slackIntegration)
		{
			this.slackIntegration = slackIntegration;
		}

        public async Task Execute(SlackMessage message)
        {
            var split = message.Text.Split(' ');
            if (split.Length != 3) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, "Did not understand message.");
                return;
            }

            var ip = split[2].Trim().TrimStart('<').TrimEnd('>');
            Configuration.KitchenServiceURL = "http://" + ip;
            await slackIntegration.SendMessageToChannel(message.ChatHub, "Changed IP to " + Configuration.KitchenServiceURL);
		}
    }
}
