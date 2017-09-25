using System.Linq;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Actions;
using Hjerpbakk.DIPSBot.Predicates;
using LightInject;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class TrondheimMessageHandler : MessageHandler
    {
        public TrondheimMessageHandler(IServiceContainer serviceContainer) : base(serviceContainer)
		{
            AddCommand<KitchenResponsibleAction>(new KitchenOverviewPredicate(), new BotMentionedPredicate());
            AddCommandListingAsUnknownCommand(new BotMentionedPredicate());
        }
    }
}
