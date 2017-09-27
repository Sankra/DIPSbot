using System;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Predicates
{
    struct RemoveEmployeePredicate : IPredicate
    {
        public string CommandText => "remove @user";

		public bool ShouldExecute(SlackMessage message)
		{
			if (message.Text.StartsWith("remove <@", StringComparison.InvariantCulture))
			{
				var commandParts = message.Text.Split(' ');
				if (commandParts.Length == 2 && commandParts[1].StartsWith("<@", StringComparison.InvariantCulture) && commandParts[1][commandParts[1].Length - 1] == '>')
				{
					return true;
				}
			}

			return false;
		}
    }
}
