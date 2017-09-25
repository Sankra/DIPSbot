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
			AddCommand<ThanksAction>(new BotMentionedPredicate(), new ThanksPredicate());
            AddCommandListingAsUnknownCommand(new BotMentionedPredicate());
		}
    }
}
