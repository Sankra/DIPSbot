using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.MessageHandlers;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    interface IAction
    {
        // TODO: Trenger jeg MessageHandler caller?
        Task Execute(SlackMessage message, MessageHandler caller);
    }
}
