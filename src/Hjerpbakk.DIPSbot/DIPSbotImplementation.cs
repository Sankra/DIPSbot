using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot.Services;
using Hjerpbakk.DIPSBot;
using Hjerpbakk.DIPSBot.Actions;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.MessageHandlers;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSbot
{
	class DIPSbotImplementation
	{
		readonly ISlackIntegration slackIntegration;
		readonly IOrganizationService organizationService;
        readonly TrondheimKitchenResponsibleAction trondheimKitchenResponsibleActions;
        readonly UserKitchenResponsibleAction userKitchenResponsibleAction;

        readonly Action<Exception> fatalExceptionHandler;
        readonly SlackUser adminUser;

		public DIPSbotImplementation(ISlackIntegration slackIntegration, 
                                     IOrganizationService organizationService, 
                                     TrondheimKitchenResponsibleAction trondheimKitchenResponsibleActions, 
                                     UserKitchenResponsibleAction userKitchenResponsibleAction,
                                     Configuration configuration)
		{
			this.slackIntegration = slackIntegration ?? throw new ArgumentNullException(nameof(slackIntegration));
			this.organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
            this.trondheimKitchenResponsibleActions = trondheimKitchenResponsibleActions ?? throw new ArgumentNullException(nameof(trondheimKitchenResponsibleActions));
            this.userKitchenResponsibleAction = userKitchenResponsibleAction ?? throw new ArgumentNullException(nameof(userKitchenResponsibleAction));
            // TODO: to nullsjekker
            fatalExceptionHandler = configuration.FatalExceptionHandler;
            adminUser = new SlackUser { Id = configuration.AdminUser };
		}

		/// <summary>
		///     Disconnects the bot from Slack.
		/// </summary>
        public async Task Close()
		{
			slackIntegration.MessageReceived -= MessageReceived;
            await slackIntegration.Close();
		}

		/// <summary>
		///     Connects the bot to Slack.
		/// </summary>
		/// <returns>No object or value is returned by this method when it completes.</returns>
		public async Task Connect()
		{
			await slackIntegration.Connect();
			slackIntegration.MessageReceived += MessageReceived;
		}

		/// <summary>
		///     Parses the messages sent to the bot and answers to the best of its abilities.
		///     Extend this method to include more commands.
		/// </summary>
		/// <param name="message">The message sent to the bot.</param>
		/// <returns>No object or value is returned by this method when it completes.</returns>
		async Task MessageReceived(SlackMessage message)
		{
            try
            {
                if (MessageIsInvalid(message)) {
                    return;
                }

                message.Text = message.Text.ToLower();
                var messageHandler = GetMessageHandler(message);
                await messageHandler.HandleMessage(message);
            }
            catch (Exception exception)
            {
                await slackIntegration.SendDirectMessage(adminUser , "I died:\n" + exception);
                fatalExceptionHandler(exception);
            }
		}

        MessageHandler GetMessageHandler(SlackMessage message) {
            if (message.ChatHub.Type == SlackChatHubType.Group) {
				if (message.ChatHub.Name == "#bot-test")
				{
					return new TrondheimMessageHandler(slackIntegration, trondheimKitchenResponsibleActions);
				}
            }

			if (message.ChatHub.Type == SlackChatHubType.Channel)
			{
			    return new ChannelMessageHandler(slackIntegration);
			}

			if (message.ChatHub.Type == SlackChatHubType.DM)
			{
                if (message.User.Id == adminUser.Id) {
                    return new AdminMessageHandler(slackIntegration, organizationService, userKitchenResponsibleAction);
                }

                return new RegularUserMessageHandler(slackIntegration, userKitchenResponsibleAction);
			}

            return new DefaultMessageHandler();
        }

		static bool MessageIsInvalid(SlackMessage message) =>
	        message == null && 
            message.User == null && 
            string.IsNullOrEmpty(message.User.Id) && 
            string.IsNullOrEmpty(message.Text) && 
            message.ChatHub == null;
	}
}