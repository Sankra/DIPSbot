﻿using Hjerpbakk.DIPSBot.Actions;
using Hjerpbakk.DIPSBot.Predicates;
using LightInject;

namespace Hjerpbakk.DIPSBot.MessageHandlers {
    class AdminMessageHandler : MessageHandler {
        public AdminMessageHandler(IServiceContainer serviceContainer) : base(serviceContainer) {
            // TODO: Get all actions and predicates...
            // TODO: Add echo functionality
            AddCommand<SwapAnswerAction>(new SwapAnswerPredicate());
            AddCommand<SwapKitchenWeekAction>(new SwapKitchenWeekPredicate());
            AddCommand<ComicsAction>(new ComicsPredicate());
            AddCommand<KitchenResponsibleAction>(new KitchenPredicate());
            AddCommand<AddEmployeeAction>(new AddEmployeePredicate());
            AddCommand<RemoveEmployeeAction>(new RemoveEmployeePredicate());
            AddCommand<WeekAction>(new WeekPredicate());
            AddCommand<AddDevelopersToUtviklingChannelAction>(new AddDevelopersToUtviklingChannelPredicate());
            AddCommand<VersionAction>(new VersionPredicate());
            AddCommand<ScooterAction>(new ScooterPredicate());
            AddCommand<BikeSharingAction>(new BikeSharingPredicate());
            AddCommandListingAsUnknownCommand(new TruePredicate());
        }
    }
}
