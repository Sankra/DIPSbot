using System;
using System.Linq;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class KitchenResponsibleAction : IAction
    {
		readonly ISlackIntegration slackIntegration;
		readonly IKitchenResponsibleClient kitchenResponsibleClient;

        public KitchenResponsibleAction(ISlackIntegration slackIntegration, IKitchenResponsibleClient kitchenResponsibleClient)
        {
            this.slackIntegration = slackIntegration;
            this.kitchenResponsibleClient = kitchenResponsibleClient;
        }

		public async Task Execute(SlackMessage message)
		{
			try
            {
                if (message.Text.Contains("neste") || message.Text.Contains("next")) {
                    // TODO: Sjekk om person blir nevnt
                    // TODO: ellers er det den som sender meldingen 
                }

				// TODO: Dersom melding har tall
				// TODO: Sjekk om person blir nevnt
				// TODO: ellers er det den som sender meldingen 

                // TODO: Denne uken

                // TODO: Ellers blir det denne

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
