using Hjerpbakk.DIPSBot.Actions;
using Hjerpbakk.DIPSBot.Predicates;
using LightInject;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class TrondheimMessageHandler : MessageHandler
    {
        public TrondheimMessageHandler(IServiceContainer serviceContainer) : base(serviceContainer)
		{
            AddCommand<ComicsAction>(new BotMentionedPredicate(), new ComicsPredicate());
            AddCommand<KitchenResponsibleAction>(new BotMentionedPredicate(), new KitchenPredicate());
            AddCommand<WeekAction>(new BotMentionedPredicate(), new WeekPredicate());
            AddCommand<ThanksAction>(new BotMentionedPredicate(), new ThanksPredicate());
            AddCommand<NegativeAction>(new BotMentionedPredicate(), new NegativePredicate());
            AddCommandListingAsUnknownCommand(new BotMentionedPredicate());
        }
    }
}
