using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSbot.Services;
using Hjerpbakk.DIPSBot.Actions;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class AdminMessageHandler : IMessageHandler
    {
        readonly ISlackIntegration slackIntegration;
        readonly IOrganizationService organizationService;
        readonly KitchenResponsibleActions kitchenResponsibleActions;

        public AdminMessageHandler(ISlackIntegration slackIntegration, IOrganizationService organizationService, KitchenResponsibleActions kitchenResponsibleActions)
        {
            this.slackIntegration = slackIntegration;
            this.organizationService = organizationService;
            this.kitchenResponsibleActions = kitchenResponsibleActions;
        }

        public async Task HandleMessage(SlackMessage message)
        {
			if (message.Text.Contains("kjøkken")) {
				await kitchenResponsibleActions.SendMessageWithKitchenResponsibles(message);
			} else if (message.Text == "utv") {
				await AddDevelopersToDeveloperChannel(message);
            } else {
                await slackIntegration.SendDirectMessage(message.User, "Unknown command");
            }

			
        }

		async Task AddDevelopersToDeveloperChannel(SlackMessage message)
		{
			await slackIntegration.IndicateTyping(message.User);
			var developers = await organizationService.GetDevelopers();
			var slackUsers = await slackIntegration.GetAllUsers();
		}
    }
}
