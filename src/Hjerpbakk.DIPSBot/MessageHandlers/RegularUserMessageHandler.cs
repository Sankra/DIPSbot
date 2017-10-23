using Hjerpbakk.DIPSBot.Actions;
using Hjerpbakk.DIPSBot.Predicates;
using LightInject;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class RegularUserMessageHandler : MessageHandler
    {
        public RegularUserMessageHandler(IServiceContainer serviceContainer) : base(serviceContainer)
		{
            AddCommand<ComicsAction>(new ComicsPredicate());
            AddCommand<KitchenResponsibleAction>(new KitchenPredicate());
            AddCommand<WeekAction>(new WeekPredicate());
            AddCommand<ThanksAction>(new ThanksPredicate());
            AddCommand<NegativeAction>(new NegativePredicate());
			AddCommandListingAsUnknownCommand(new TruePredicate());
		}
    }
}
