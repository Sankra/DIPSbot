using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Model;
using Hjerpbakk.DIPSBot.Clients;
using SlackConnector.Models;
using System.Text.RegularExpressions;
using Hjerpbakk.DIPSBot.Configuration;

namespace Hjerpbakk.DIPSBot.Actions
{
    class KitchenResponsibleAction : IAction
    {
        readonly ISlackIntegration slackIntegration;
		readonly IKitchenResponsibleClient kitchenResponsibleClient;
        readonly IReadOnlyAppConfiguration configuration;

        readonly Regex numberInStringRegex;
        readonly Regex slackUserRegex;

        public KitchenResponsibleAction(ISlackIntegration slackIntegration, IKitchenResponsibleClient kitchenResponsibleClient, IReadOnlyAppConfiguration configuration)
        {
            this.slackIntegration = slackIntegration;
            this.kitchenResponsibleClient = kitchenResponsibleClient;
            this.configuration = configuration;

            numberInStringRegex = new Regex(@"\b\d+", RegexOptions.Compiled);
            slackUserRegex = new Regex(@"<@(\S{9})>", RegexOptions.Compiled);
        }

        // TODO: Vi har uendelig mange string contains, dette kan gjøres mye mer sm00th. Gjelder predicater også...
		public async Task Execute(SlackMessage message)
		{
			try
            {
                string numberInString = null;
                if (message.Text.Contains("denne")) {
                    var employeeAndWeek = await kitchenResponsibleClient.GetResponsibleForCurrentWeek();
                    await SendWeekAndResponsible(message.ChatHub, employeeAndWeek);
                } else if (message.Text.Contains("neste") || message.Text.Contains("når")) {
					if (message.ChatHub.Type == SlackChatHubType.DM || message.Text.Contains("jeg")) {
                        await SendMessageWithNextWeekForUser(message.ChatHub, message.User);
                        return;
                    }

                    string slackUserToCheck = null;
                    Match match = slackUserRegex.Match(message.Text);
                    while (match.Success)
                    {
                        if (match.Groups.Count == 2) {
                            var slackUserId = match.Groups[1].Value;
                            if (!slackUserId.Equals(configuration.BotUser, StringComparison.InvariantCultureIgnoreCase)) {
                                slackUserToCheck = slackUserId.ToUpper();
                                break;
                            }
                        } 

                        match = match.NextMatch();
                    }

                    if (slackUserToCheck != null) {
                        var slackUser = new SlackUser { Id = slackUserToCheck };
                        await SendMessageWithNextWeekForUser(message.ChatHub, slackUser);
                    } else {
                        var thisWeek = EmployeeWeek.GetIso8601WeekOfYear(DateTime.Now);
                        var nextWeek = EmployeeWeek.GetNextWeek(thisWeek);
                        await SendMessageForWeek(message.ChatHub, nextWeek);
                    }				
                } else if ((numberInString = numberInStringRegex.Match(message.Text).Value) != "") {
                    var number = int.Parse(numberInString);
					if (number <= 0 || number > 53)	{
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
                    await SendMessageForWeek(message.ChatHub, weekNumber);
                } else {
    				var employeesAndWeeks = await kitchenResponsibleClient.GetAllWeeks();
    				var kitchenResponsibleTable = string.Join("\n", employeesAndWeeks.Select(w => w.FormattedEmployeeWeek));
                    var kitchenResponsible = $"*Kjøkkenansvarlig*\n{kitchenResponsibleTable}\n\n<{configuration.KitchenServiceURL}|Kjøkkenansvarligoversikt på nett>";
    				await slackIntegration.SendMessageToChannel(message.ChatHub, kitchenResponsible);
                }
            }
            catch (Exception ex)
            {
                await slackIntegration.SendMessageToChannel(message.ChatHub, ex.ToString());
            }
		}

        async Task SendMessageWithNextWeekForUser(SlackChatHub chatHub, SlackUser slackUser)
		{
            var employeeAndWeek = await kitchenResponsibleClient.GetNextWeekForEmployee(slackUser);
            if (employeeAndWeek.WeekNumber == 0) {
				await slackIntegration.SendMessageToChannel(chatHub,
                    $"Fant ingen uker der {slackUser.FormattedUserId} er kjøkkenansvarlig.");
            } else {
                await SendWeekAndResponsible(chatHub, employeeAndWeek);
            }
		}

        async Task SendMessageForWeek(SlackChatHub chatHub, ushort nextWeek)
        {
            var employeeAndWeek = await kitchenResponsibleClient.GetResponsibleForWeek(nextWeek);
            if (employeeAndWeek.SlackUser.Id == null) {
                await slackIntegration.SendMessageToChannel(chatHub,
                    $"Ingen er kjøkkenansvarlig ennå for uke {employeeAndWeek.WeekNumber}.");
            }
            else {
                await SendWeekAndResponsible(chatHub, employeeAndWeek);
            }
        }

        async Task SendWeekAndResponsible(SlackChatHub chatHub, EmployeeWeek employeeAndWeek) =>
			await slackIntegration.SendMessageToChannel(chatHub,
					$"Kjøkkenansvarlig for uke {employeeAndWeek.WeekNumber} er {employeeAndWeek.SlackUser.FormattedUserId}.");
    }
}
