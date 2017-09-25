using System.Linq;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Actions;
using Hjerpbakk.DIPSBot.Predicates;
using LightInject;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class AdminMessageHandler : MessageHandler
    {
        public AdminMessageHandler(IServiceContainer serviceContainer) : base(serviceContainer)
        {
			AddCommand<KitchenResponsibleAction>(new KitchenPredicate());
            AddCommand<AddDevelopersToUtviklingChannelAction>(new AddEmployeePredicate());
            AddCommand<AddEmployeeAction>(new AddDevelopersToUtviklingChannelPredicate());
			AddCommandListingAsUnknownCommand(new TruePredicate());
        }
    }
}
