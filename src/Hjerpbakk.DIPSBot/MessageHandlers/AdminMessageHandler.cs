using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSbot.Services;
using Hjerpbakk.DIPSBot.Actions;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class AdminMessageHandler : MessageHandler
    {
        readonly ISlackIntegration slackIntegration;
        readonly IOrganizationService organizationService;
        readonly UserKitchenResponsibleAction kitchenResponsibleActions;

        public AdminMessageHandler(ISlackIntegration slackIntegration, IOrganizationService organizationService, UserKitchenResponsibleAction userKitchenResponsibleActions)
        {
            this.slackIntegration = slackIntegration;
            this.organizationService = organizationService;
            this.kitchenResponsibleActions = userKitchenResponsibleActions;

            actions.Add(userKitchenResponsibleActions);
            // TODO: Inn i containeren med både handlers og actions...
            actions.Add(new AddDevelopersToUtviklingChannelAction(slackIntegration, organizationService));
            actions.Add(new ListCommandsAction(slackIntegration, actions));
        }
    }
}
