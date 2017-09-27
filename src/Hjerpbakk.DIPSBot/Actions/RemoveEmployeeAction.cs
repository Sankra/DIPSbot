using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Predicates;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class RemoveEmployeeAction : IAction
    {
        readonly ISlackIntegration slackIntegration;
        readonly IKitchenResponsibleClient kitchenResponsibleClient;

		public RemoveEmployeeAction(ISlackIntegration slackIntegration, IKitchenResponsibleClient kitchenResponsibleClient)
        {
            this.slackIntegration = slackIntegration;
            this.kitchenResponsibleClient = kitchenResponsibleClient;
        }

        public AddEmployeePredicate AddEmployeePredicate { private get; set; }

        public async Task Execute(SlackMessage message)
        {
            try
            {
                var commandParts = message.Text.Split(' ');
				var slackUserId = commandParts[1].Substring(2, commandParts[1].Length - 3).ToUpper();
				var userToRemove = new SlackUser { Id = slackUserId };
                await slackIntegration.SendMessageToChannel(message.ChatHub, $"Removing {userToRemove.FormattedUserId} as employee...");
				await kitchenResponsibleClient.RemoveEmployee(userToRemove);
				await slackIntegration.SendMessageToChannel(message.ChatHub, $"{userToRemove.FormattedUserId} was removed.");
            }
            catch (Exception ex)
            {
                await slackIntegration.SendMessageToChannel(message.ChatHub, ex.ToString());
            }
        }
    }
}
