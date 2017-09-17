using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSbot.Services;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class AdminMessageHandler : IMessageHandler
    {
        readonly ISlackIntegration slackIntegration;
        readonly IOrganizationService organizationService;

        public AdminMessageHandler(ISlackIntegration slackIntegration, IOrganizationService organizationService)
        {
            this.slackIntegration = slackIntegration;
            this.organizationService = organizationService;
        }

        public async Task HandleMessage(SlackMessage message)
        {
			if (message.Text != "utv")
			{
				await slackIntegration.SendDirectMessage(message.User, "Unknown command");
				return;
			}

			await AddDevelopersToDeveloperChannel(message);
        }

		async Task AddDevelopersToDeveloperChannel(SlackMessage message)
		{
			await slackIntegration.IndicateTyping(message.User);
			var developers = await organizationService.GetDevelopers();
			var slackUsers = await slackIntegration.GetAllUsers();
		}
    }
}
