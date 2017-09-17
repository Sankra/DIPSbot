using System;
using System.Threading.Tasks;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    interface IMessageHandler
    {
        Task HandleMessage(SlackMessage message);
    }
}
