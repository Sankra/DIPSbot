using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Actions;
using Hjerpbakk.DIPSBot.Predicates;
using LightInject;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    abstract class MessageHandler
    {
        readonly IServiceContainer serviceContainer;
        readonly List<(IAction action, IPredicate[] predicates)> commands;

        public MessageHandler(IServiceContainer serviceContainer)
        {
            this.serviceContainer = serviceContainer;
            commands = new List<(IAction action, IPredicate[] predicates)>();
        }

        public async Task<string> HandleMessage(SlackMessage message) {

            foreach (var command in commands)
            {
                if (command.predicates.All(p => p.ShouldExecute(message))) {
                    await command.action.Execute(message, this);
                    return command.action.GetType().Name;
                }
            }

            return "No action taken";
        }

        protected void AddCommand<T>(params IPredicate[] predicates) {
            var action = (IAction)serviceContainer.GetInstance<T>();
            commands.Add((action, predicates));
        }

        protected void AddCommandListingAsUnknownCommand(IPredicate listCommandsPredicate) {
            var potentionalCommands = commands.Select(c => c.predicates.First(p => !(p is BotMentionedPredicate)));
            var availableCommands = potentionalCommands.Where(c => c.CommandText != "").ToArray();
            var commandsString = string.Join("", availableCommands.Select(predicate => "- " + predicate.CommandText + "\n"));
            commands.Add((new ListCommandsAction(serviceContainer.GetInstance<ISlackIntegration>(), commandsString), new[] { listCommandsPredicate }));
        }
    }
}
