using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSbot.Services;
using Hjerpbakk.DIPSBot.MessageHandlers;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class AddDevelopersToUtviklingChannelAction : IAction
    {
		readonly ISlackIntegration slackIntegration;
		readonly IOrganizationService organizationService;

        public AddDevelopersToUtviklingChannelAction(ISlackIntegration slackIntegration, IOrganizationService organizationService)
		{
			this.slackIntegration = slackIntegration;
			this.organizationService = organizationService;
        }

        public Task Execute(SlackMessage message, MessageHandler caller)
        {
            //await slackIntegration.IndicateTyping(message.User);
            //var developers = await organizationService.GetDevelopers();
            //var slackUsers = await slackIntegration.GetAllUsers();
            return Task.CompletedTask;
        }
    }
}
