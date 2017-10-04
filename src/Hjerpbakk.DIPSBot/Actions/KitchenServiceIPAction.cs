using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Configuration;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    public class KitchenServiceIPAction : IAction
    {
		readonly ISlackIntegration slackIntegration;
        readonly IReadOnlyAppConfiguration configuration;
        readonly ServiceDiscoveryClient serviceDiscoveryClient;

        public KitchenServiceIPAction(ISlackIntegration slackIntegration, ServiceDiscoveryClient serviceDiscoveryClient, IReadOnlyAppConfiguration configuration)
		{
			this.slackIntegration = slackIntegration;
            this.configuration = configuration;
            this.serviceDiscoveryClient = serviceDiscoveryClient;
		}

        public async Task Execute(SlackMessage message)
        {
			var split = message.Text.Split(' ');
            if (split.Length == 2) {
                await slackIntegration.SendMessageToChannel(message.ChatHub, "Kitchen Service IP is " + configuration.KitchenServiceURL);
			} else if (split.Length == 3) {
				var ip = split[2].Trim().TrimStart('<').TrimEnd('>');
                await serviceDiscoveryClient.UploadNewKitchenServiceURL(ip);
                await serviceDiscoveryClient.SetKitchenServiceURL();
				await slackIntegration.SendMessageToChannel(message.ChatHub, "Changed Kitchen Service IP to " + configuration.KitchenServiceURL);    
            } else {
                await slackIntegration.SendMessageToChannel(message.ChatHub, "Did not understand message.");   
            }
		}
    }
}
