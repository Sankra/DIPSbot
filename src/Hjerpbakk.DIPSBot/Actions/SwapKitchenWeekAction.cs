using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.MessageHandlers;
using Hjerpbakk.DIPSBot.Model;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class SwapKitchenWeekAction : IAction
    {
        readonly ISlackIntegration slackIntegration;
        readonly IKitchenResponsibleClient kitchenResponsibleClient;

        readonly Regex slackUserRegex;

        public SwapKitchenWeekAction(ISlackIntegration slackIntegration, IKitchenResponsibleClient kitchenResponsibleClient)
        {
            this.slackIntegration = slackIntegration;
            this.kitchenResponsibleClient = kitchenResponsibleClient;

            slackUserRegex = new Regex(@"<@(\S{9})>", RegexOptions.Compiled);
        }

        public async Task Execute(SlackMessage message, MessageHandler caller)
        {
            string userToSwapWith = null;
            string userIdToSwapWith = null;
            Match match = slackUserRegex.Match(message.Text);
            while (match.Success)
            {
                if (match.Groups.Count == 2)
                {
                    userIdToSwapWith = match.Groups[0].Value;
                    userToSwapWith = userIdToSwapWith.ToUpper();
                    break;
                }

                match = match.NextMatch();
            }

            if (userToSwapWith == null)
            {
                await slackIntegration.SendMessageToChannel(message.ChatHub, "Kunne ikke finne bruker å bytte med i meldingen.");
                return;
            }

            var swap = new Swap { FirstSwapperId = message.User.Id, SecondSwapperId = userIdToSwapWith };
            await slackIntegration.SendMessageToChannel(message.ChatHub, $"Spør {userToSwapWith} om å bytte vakt, hang tight.");
            // TODO: Sende til tjenesten at bytte ønskes
        }
    }
}
