﻿using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
    struct ScooterPredicate : IPredicate
    {
        public string CommandText => "scooter";

        public bool ShouldExecute(SlackMessage message) =>
            message.Text.Contains("scooter");
    }
}
