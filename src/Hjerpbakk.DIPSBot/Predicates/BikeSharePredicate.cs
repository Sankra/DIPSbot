﻿using System;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates {
    readonly struct BikeSharePredicate : IPredicate {
        public string CommandText => "([pick-up | drop-off]) bike";

        public bool ShouldExecute(SlackMessage message) =>
            message.Text.Contains("bike") || (message.Text.Contains("sykkel"));
    }
}


