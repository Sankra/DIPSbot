﻿using Hjerpbakk.DIPSBot.Actions;
using Hjerpbakk.DIPSBot.Predicates;
using LightInject;

namespace Hjerpbakk.DIPSBot.MessageHandlers {
    class TrondheimMessageHandler : MessageHandler {
        // TODO: Karsklag should also get this
        public TrondheimMessageHandler(IServiceContainer serviceContainer) : base(serviceContainer) {
            // TODO: Trondheim is superset of channel
            AddCommand<ComicsAction>(new BotMentionedPredicate(), new ComicsPredicate());
            AddCommand<KitchenResponsibleAction>(new BotMentionedPredicate(), new KitchenPredicate());
            AddCommand<WeekAction>(new BotMentionedPredicate(), new WeekPredicate());
            AddCommand<ThanksAction>(new BotMentionedPredicate(), new ThanksPredicate());
            AddCommand<NegativeAction>(new BotMentionedPredicate(), new NegativePredicate());
            AddCommand<VersionAction>(new BotMentionedPredicate(), new VersionPredicate());
            AddCommand<ScooterAction>(new BotMentionedPredicate(), new ScooterPredicate());
            AddCommand<BikeSharingAction>(new BotMentionedPredicate(), new BikeSharingPredicate());
            AddCommandListingAsUnknownCommand(new BotMentionedPredicate());
        }
    }
}
