using System.Threading.Tasks;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    interface IAction
    {
        string CommandText { get; }

        bool ShouldExecute(SlackMessage message);
        Task Execute(SlackMessage message);
    }
}
