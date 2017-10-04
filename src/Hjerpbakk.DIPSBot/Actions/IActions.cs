using System.Threading.Tasks;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    interface IAction
    {
        // TODO: Legg inn  muligheten for å både se (for vanlige brukere) og oppdatere hvilke tegneserier som skal vises..
        Task Execute(SlackMessage message);
    }
}
