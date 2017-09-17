using System;
using System.Linq;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class TrondheimMessageHandler : IMessageHandler
    {
        readonly ISlackIntegration slackIntegration;
        readonly IKitchenResponsibleClient kitchenResponsibleClient;

        public TrondheimMessageHandler(ISlackIntegration slackIntegration, IKitchenResponsibleClient kitchenResponsibleClient)
        {
            this.slackIntegration = slackIntegration;
            this.kitchenResponsibleClient = kitchenResponsibleClient;
        }

        public async Task HandleMessage(SlackMessage message)
        {
            if (message.MentionsBot == true) {
                var normalizedText = message.Text.ToLower();
                if (normalizedText.Contains("kjøkken"))
                {
                    //await slackIntegration.SendMessageToChannel(message.ChatHub, "Alle andre enn Runar.");    
                    await GetAllKitchenResponsibles(message);
                }
            }
        }

        async Task GetAllKitchenResponsibles(SlackMessage message)
        {
            var employeesAndWeeks = await kitchenResponsibleClient.GetAllWeeks();
            var kitchenResponsibleTable = string.Join("\n", employeesAndWeeks.Select(w => w.WeekNumber + ". " + w.SlackUser.FormattedUserId));
            var kitchenResponsible = "*Kjøkkenansvarlig*\n" + kitchenResponsibleTable;
            await slackIntegration.SendMessageToChannel(message.ChatHub, kitchenResponsible);
        }
    }
}
