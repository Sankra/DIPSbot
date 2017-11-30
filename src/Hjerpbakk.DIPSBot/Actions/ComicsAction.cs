using System.Threading.Tasks;
using Hjerpbakk.DIPSbot;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.MessageHandlers;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Actions
{
    class ComicsAction : IAction
	{
		readonly ISlackIntegration slackIntegration;
        readonly ComicsClient comicsClient;

		public ComicsAction(ISlackIntegration slackIntegration, ComicsClient comicsClient)
		{
			this.slackIntegration = slackIntegration;
            this.comicsClient = comicsClient;
		}

        // TODO: Legg inn  muligheten for å både se (for vanlige brukere) og oppdatere hvilke tegneserier som skal vises..
        public async Task Execute(SlackMessage message, MessageHandler caller) {
            var comicURL = await comicsClient.GetRandomComicAsync();
            var comic = new SlackAttachment { ImageUrl = comicURL };
            await slackIntegration.SendMessageToChannel(message.ChatHub, "Awesome tegneserie \ud83d\ude03", comic);
        }
    }
}
