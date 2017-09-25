using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Model;
using Hjerpbakk.DIPSBot.Clients;
using SlackConnector.Models;
using System.Text.RegularExpressions;

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
                string numberInString = null;
                if (message.Text.Contains("denne")) {
                    var employeeAndWeek = await kitchenResponsibleClient.GetResponsibleForCurrentWeek();
                    var kitchenResponsible = $"Kjøkkenansvarlig for uke {employeeAndWeek.WeekNumber} er {employeeAndWeek.SlackUser.FormattedUserId}.";
					await slackIntegration.SendMessageToChannel(message.ChatHub, kitchenResponsible);
                } else if (message.Text.Contains("neste")) {
					// TODO: Sjekk om person blir nevnt, da skal vi finne denne personens neste uke
					if (message.Text.Contains("uke"))
                    {
                        var thisWeek = EmployeeWeek.GetIso8601WeekOfYear(DateTime.Now);
                        var nextWeek = EmployeeWeek.GetNextWeek(thisWeek);
                        await SendMessageForWeek(message, nextWeek);
                    }
                    else {
						// TODO: Må også støtte når en person spør direktenår denne personens neste uke blir...
					}
					
                } else if ((numberInString = Regex.Match(message.Text, @"\d+").Value) != null) {
                    var number = int.Parse(numberInString);
					if (number <= 0 || number > 53)
					{
						await slackIntegration.SendMessageToChannel(message.ChatHub, 
                            $"Uke {number} er ikke en god uke for kjøkkenjobbing.");
                        return;
                    }  
                    
                    if (number == 53) {
						await slackIntegration.SendMessageToChannel(message.ChatHub,
							$"Den som har kjøkkenet i uke 52, har det også i uke 53.");
                        number = 52;
                    } 
                        
                    var weekNumber = (ushort)number;
                    await SendMessageForWeek(message, weekNumber);

						
						
					


                }

				

				// TODO: Sjekk om person blir nevnt
				// TODO: ellers er det den som sender meldingen 



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

        async Task SendMessageForWeek(SlackMessage message, ushort nextWeek)
        {
            var employeeAndWeek = await kitchenResponsibleClient.GetResponsibleForWeek(nextWeek);
            if (employeeAndWeek.SlackUser.Id == null)
            {
                await slackIntegration.SendMessageToChannel(message.ChatHub,
                    $"Ingen er kjøkkenansvarlig ennå for uke {employeeAndWeek.WeekNumber}.");
            }
            else
            {
                await slackIntegration.SendMessageToChannel(message.ChatHub,
                    $"Kjøkkenansvarlig for uke {employeeAndWeek.WeekNumber} er {employeeAndWeek.SlackUser.FormattedUserId}.");
            }
        }
    }
}
