using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Model;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    public class WeekAction : IAction
    {
		readonly ISlackIntegration slackIntegration;
		
		public WeekAction(ISlackIntegration slackIntegration)
		{
			this.slackIntegration = slackIntegration;
		}

        public async Task Execute(SlackMessage message) =>
        await slackIntegration.SendMessageToChannel(message.ChatHub, $"Denne uken har ukenummer {EmployeeWeek.GetIso8601WeekOfYear(DateTime.Now)}.");
    }
}
