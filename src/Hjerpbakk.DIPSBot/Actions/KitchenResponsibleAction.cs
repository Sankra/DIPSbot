using System;
using System.Linq;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    abstract class KitchenResponsibleAction
    {
		readonly ISlackIntegration slackIntegration;
		readonly IKitchenResponsibleClient kitchenResponsibleClient;

        protected KitchenResponsibleAction(ISlackIntegration slackIntegration, IKitchenResponsibleClient kitchenResponsibleClient)
        {
            this.slackIntegration = slackIntegration;
            this.kitchenResponsibleClient = kitchenResponsibleClient;
        }

		public async Task Execute(SlackMessage message)
		{
            try
            {
				// TODO: Får StackOverflow Exception når server returnerer crash...
				var employeesAndWeeks = await kitchenResponsibleClient.GetAllWeeks();
				var kitchenResponsibleTable = string.Join("\n", employeesAndWeeks.Select(w => w.WeekNumber + ". " + w.SlackUser.FormattedUserId));
				var kitchenResponsible = "*Kjøkkenansvarlig*\n" + kitchenResponsibleTable;
				await slackIntegration.SendMessageToChannel(message.ChatHub, kitchenResponsible);
            }
            catch (Exception ex)
            {
                await slackIntegration.SendMessageToChannel(message.ChatHub, ex.ToString());
            }
		}
    }
}
