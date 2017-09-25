using System.Threading.Tasks;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    interface IAction
    {
        Task Execute(SlackMessage message);
    }
}
