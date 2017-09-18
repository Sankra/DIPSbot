using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSbot.Services;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    public class AddDevelopersToUtviklingChannelAction : IAction
    {
		readonly ISlackIntegration slackIntegration;
		readonly IOrganizationService organizationService;

        public AddDevelopersToUtviklingChannelAction(ISlackIntegration slackIntegration, IOrganizationService organizationService)
		{
			this.slackIntegration = slackIntegration;
			this.organizationService = organizationService;
        }

        public string CommandText => "utv";

        public async Task Execute(SlackMessage message)
        {
			await slackIntegration.IndicateTyping(message.User);
			var developers = await organizationService.GetDevelopers();
			var slackUsers = await slackIntegration.GetAllUsers();
        }

        public bool ShouldExecute(SlackMessage message) => message.Text == "utv";
    }
}
