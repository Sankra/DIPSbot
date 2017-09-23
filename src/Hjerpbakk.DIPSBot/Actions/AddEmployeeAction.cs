using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class AddEmployeeAction : IAction
    {
        readonly ISlackIntegration slackIntegration;
        readonly IKitchenResponsibleClient kitchenResponsibleClient;

        SlackUser userToAdd;

		public AddEmployeeAction(ISlackIntegration slackIntegration, IKitchenResponsibleClient kitchenResponsibleClient)
        {
            this.slackIntegration = slackIntegration;
            this.kitchenResponsibleClient = kitchenResponsibleClient;
        }

        public string CommandText => "add @user";

        public async Task Execute(SlackMessage message)
        {
            try
            {
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

        public bool ShouldExecute(SlackMessage message) {
            if (message.Text.StartsWith("add <@", StringComparison.InvariantCulture)) {
                var commandParts = message.Text.Split(' ');
				if (commandParts.Length == 2 && commandParts[1].StartsWith("<@", StringComparison.InvariantCulture) && commandParts[1][commandParts[1].Length - 1] == '>')
				{
					var slackUserId = commandParts[1].Substring(2, commandParts[1].Length - 3).ToUpper();
					userToAdd = new SlackUser { Id = slackUserId };
                    return true;
				}
            }

            return false;
        }
    }
}
