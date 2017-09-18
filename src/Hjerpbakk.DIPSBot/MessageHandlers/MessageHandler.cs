using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Actions;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    abstract class MessageHandler
    {
        protected readonly List<IAction> actions;

        protected MessageHandler()
        {
            actions = new List<IAction>();
        }

        public async Task HandleMessage(SlackMessage message) {

            foreach (var action in actions)
            {
                if (action.ShouldExecute(message)) {
                    await action.Execute(message);
                    break;
                }
            }
        }
    }
}
