using System;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
	public struct ChangeKitchenServiceIPPredicate : IPredicate
	{
		public string CommandText => "kitchen ip";

        public bool ShouldExecute(SlackMessage message) => message.Text.StartsWith(CommandText, StringComparison.InvariantCultureIgnoreCase);
	}
}
