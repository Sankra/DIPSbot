using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.MessageHandlers;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class SwapAnswerAction : IAction
    {
        public SwapAnswerAction()
        {
        }

        public Task Execute(SlackMessage message, MessageHandler caller)
        {
            // TODO: Sjekk tjenesten om bytte er på tapeten
            // TODO: Nei = Slett innslaget
            // TODO: Ja = slett innslaget og gjennomfør byttingen
            // TODO: Svar begge brukerne om utfallet
            throw new NotImplementedException();
        }
    }
}
