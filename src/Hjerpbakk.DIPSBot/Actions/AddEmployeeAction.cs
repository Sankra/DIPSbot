using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Predicates;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class AddEmployeeAction : IAction
    {
        readonly ISlackIntegration slackIntegration;
        readonly IKitchenResponsibleClient kitchenResponsibleClient;

		public AddEmployeeAction(ISlackIntegration slackIntegration, IKitchenResponsibleClient kitchenResponsibleClient)
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
				var userToAdd = new SlackUser { Id = slackUserId };
				await slackIntegration.SendMessageToChannel(message.ChatHub, $"Adding {userToAdd.FormattedUserId} as employee...");
				var result = await kitchenResponsibleClient.AddEmployee(userToAdd);
				if (result.ok)
				{
					await slackIntegration.SendMessageToChannel(message.ChatHub, $"{userToAdd.FormattedUserId} was added.");
				}
				else
				{
					await slackIntegration.SendMessageToChannel(message.ChatHub, result.error);
				}
            }
            catch (Exception ex)
            {
                await slackIntegration.SendMessageToChannel(message.ChatHub, ex.ToString());
            }
        }
    }
}
