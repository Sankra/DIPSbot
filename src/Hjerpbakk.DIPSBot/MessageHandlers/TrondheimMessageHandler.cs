using System;
using System.Linq;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Actions;
using Hjerpbakk.DIPSBot.Clients;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class TrondheimMessageHandler : MessageHandler
    {
        readonly ISlackIntegration slackIntegration;
        readonly TrondheimKitchenResponsibleAction kitchenResponsibleActions;

        public TrondheimMessageHandler(ISlackIntegration slackIntegration, TrondheimKitchenResponsibleAction kitchenResponsibleActions)
        {
            this.slackIntegration = slackIntegration;
            this.kitchenResponsibleActions = kitchenResponsibleActions;

            actions.Add(kitchenResponsibleActions);
        }
    }
}
