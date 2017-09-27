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
            AddCommand<AddDevelopersToUtviklingChannelAction>(new AddDevelopersToUtviklingChannelPredicate());
            AddCommand<AddEmployeeAction>(new AddEmployeePredicate());
            AddCommand<WeekAction>(new WeekPredicate());
			AddCommandListingAsUnknownCommand(new TruePredicate());
        }
    }
}
