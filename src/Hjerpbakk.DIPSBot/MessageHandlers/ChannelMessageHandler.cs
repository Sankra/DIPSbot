using System.Linq;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Actions;
using Hjerpbakk.DIPSBot.Predicates;
using LightInject;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class ChannelMessageHandler : MessageHandler
    {
        public ChannelMessageHandler(IServiceContainer serviceContainer) : base(serviceContainer)
		{
            AddCommand<ComicsAction>(new BotMentionedPredicate(), new ComicsPredicate());
			AddCommand<ThanksAction>(new BotMentionedPredicate(), new ThanksPredicate());
            AddCommand<NegativeAction>(new BotMentionedPredicate(), new NegativePredicate());
            AddCommand<VersionAction>(new BotMentionedPredicate(), new VersionPredicate());
            AddCommandListingAsUnknownCommand(new BotMentionedPredicate());
		}
    }
}
