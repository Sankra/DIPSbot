using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Model;
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
                if (message.Text.Contains("denne")) {
                    var employeeAndWeek = await kitchenResponsibleClient.GetResponsibleForCurrentWeek();
                    var kitchenResponsible = $"Kjøkkenansvarlig for uke {employeeAndWeek.WeekNumber} er {employeeAndWeek.SlackUser.FormattedUserId}.";
					await slackIntegration.SendMessageToChannel(message.ChatHub, kitchenResponsible);
                } else if (message.Text.Contains("neste")) {
                    var thisWeek = GetIso8601WeekOfYear(DateTime.Now);
                    var nextWeek = GetNextWeek(thisWeek);
                    var employeeAndWeek = await kitchenResponsibleClient.GetResponsibleForWeek(nextWeek);
                    if (employeeAndWeek.SlackUser.Id == null) {
                        await slackIntegration.SendMessageToChannel(message.ChatHub, 
                            $"Ingen er kjøkkenansvarlig ennå for uke {employeeAndWeek.WeekNumber}.");
                    } else {
                        await slackIntegration.SendMessageToChannel(message.ChatHub, 
                            $"Kjøkkenansvarlig for uke {employeeAndWeek.WeekNumber} er {employeeAndWeek.SlackUser.FormattedUserId}.");
                    }
                }

				// TODO: Dersom melding har tall
				// TODO: Sjekk om person blir nevnt
				// TODO: ellers er det den som sender meldingen 

				// TODO: Sjekk om person blir nevnt
				// TODO: ellers er det den som sender meldingen 

				// TODO: Ellers blir det denne

				// TODO: Får StackOverflow Exception når server returnerer crash...
				else {
    				var employeesAndWeeks = await kitchenResponsibleClient.GetAllWeeks();
    				var kitchenResponsibleTable = string.Join("\n", employeesAndWeeks.Select(w => w.FormattedEmployeeWeek));
                    var kitchenResponsible = $"*Kjøkkenansvarlig*\n{kitchenResponsibleTable}";
    				await slackIntegration.SendMessageToChannel(message.ChatHub, kitchenResponsible);
                }
            }
            catch (Exception ex)
            {
                await slackIntegration.SendMessageToChannel(message.ChatHub, ex.ToString());
            }
		}

        static ushort GetIso8601WeekOfYear(DateTime time)
		{
			// Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
			// be the same week# as whatever Thursday, Friday or Saturday are,
			// and we always get those right
			DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
			if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
			{
				time = time.AddDays(3);
			}

			// Return the week of our adjusted day
			return (ushort)CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
		}

		static ushort GetNextWeek(ushort week) =>
			week == 52 ? (ushort)1 : (ushort)(week + 1);

		static ushort GetPreviousWeek(ushort week) =>
			week == 1 ? (ushort)52 : (ushort)(week - 1);
    }
}
