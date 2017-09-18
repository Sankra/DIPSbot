using System;
using System.Linq;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class KitchenResponsibleActions
    {
		readonly ISlackIntegration slackIntegration;
		readonly IKitchenResponsibleClient kitchenResponsibleClient;

		public KitchenResponsibleActions(ISlackIntegration slackIntegration, IKitchenResponsibleClient kitchenResponsibleClient)
		{
			this.slackIntegration = slackIntegration;
			this.kitchenResponsibleClient = kitchenResponsibleClient;
		}

		public async Task SendMessageWithKitchenResponsibles(SlackMessage message)
		{
			var employeesAndWeeks = await kitchenResponsibleClient.GetAllWeeks();
			var kitchenResponsibleTable = string.Join("\n", employeesAndWeeks.Select(w => w.WeekNumber + ". " + w.SlackUser.FormattedUserId));
			var kitchenResponsible = "*Kjøkkenansvarlig*\n" + kitchenResponsibleTable;
			await slackIntegration.SendMessageToChannel(message.ChatHub, kitchenResponsible);
		}
    }
}
