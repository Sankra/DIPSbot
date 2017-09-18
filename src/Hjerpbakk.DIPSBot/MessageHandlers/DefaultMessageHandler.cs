using System.Threading.Tasks;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class DefaultMessageHandler : IMessageHandler
    {
        public Task HandleMessage(SlackMessage message) =>
            Task.CompletedTask;
    }
}
