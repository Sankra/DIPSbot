using System.Linq;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Actions;
using Hjerpbakk.DIPSBot.Predicates;
using LightInject;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class RegularUserMessageHandler : MessageHandler
    {
        public RegularUserMessageHandler(IServiceContainer serviceContainer) : base(serviceContainer)
		{
            AddCommand<KitchenResponsibleAction>(new KitchenOverviewPredicate());
			AddCommandListingAsUnknownCommand(new TruePredicate());
		}
    }
}
