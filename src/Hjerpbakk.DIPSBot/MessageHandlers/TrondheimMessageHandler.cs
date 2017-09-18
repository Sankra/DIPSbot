using System;
using System.Linq;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Actions;
using Hjerpbakk.DIPSBot.Clients;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class TrondheimMessageHandler : IMessageHandler
    {
        readonly ISlackIntegration slackIntegration;
        readonly KitchenResponsibleActions kitchenResponsibleActions;

        public TrondheimMessageHandler(ISlackIntegration slackIntegration, KitchenResponsibleActions kitchenResponsibleActions)
        {
            this.slackIntegration = slackIntegration;
            this.kitchenResponsibleActions = kitchenResponsibleActions;
        }

        public async Task HandleMessage(SlackMessage message)
        {
            if (message.MentionsBot == true) {
                if (message.Text.Contains("kjøkken"))
                {
                    await kitchenResponsibleActions.SendMessageWithKitchenResponsibles(message);
                }
            }
        }


    }
}
