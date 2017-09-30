using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    public class KitchenServiceIPAction : IAction
    {
		readonly ISlackIntegration slackIntegration;

		public KitchenServiceIPAction(ISlackIntegration slackIntegration)
		{
			this.slackIntegration = slackIntegration;
		}

        public async Task Execute(SlackMessage message)
        {
            var split = message.Text.Split(' ');
            if (split.Length == 2) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, "Kitchen Service IP is " + Configuration.KitchenServiceURL);
			} else if (split.Length == 3) {
				var ip = split[2].Trim().TrimStart('<').TrimEnd('>');
				Configuration.KitchenServiceURL = "http://" + ip;
				await slackIntegration.SendMessageToChannel(message.ChatHub, "Changed Kitchen Service IP to " + Configuration.KitchenServiceURL);    
            } else {
                await slackIntegration.SendMessageToChannel(message.ChatHub, "Did not understand message.");   
            }


		}
    }
}
