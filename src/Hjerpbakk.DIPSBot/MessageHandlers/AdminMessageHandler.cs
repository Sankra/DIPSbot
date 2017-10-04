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
            // TODO: Add echo functionality
			AddCommand<KitchenResponsibleAction>(new KitchenPredicate());
            AddCommand<AddEmployeeAction>(new AddEmployeePredicate());
            AddCommand<RemoveEmployeeAction>(new RemoveEmployeePredicate());
            AddCommand<WeekAction>(new WeekPredicate());
            AddCommand<AddDevelopersToUtviklingChannelAction>(new AddDevelopersToUtviklingChannelPredicate());
			AddCommandListingAsUnknownCommand(new TruePredicate());
        }
    }
}
