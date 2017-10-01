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
using LightInject;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSbot
{
    class DIPSbotImplementation
    {
        readonly IServiceContainer serviceContainer;
        readonly ISlackIntegration slackIntegration;

        readonly Action<Exception> fatalExceptionHandler;
        readonly SlackUser adminUser;

        public DIPSbotImplementation(IServiceContainer serviceContainer)
        {
            this.serviceContainer = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));

            slackIntegration = serviceContainer.GetInstance<ISlackIntegration>();
            var configuration = serviceContainer.GetInstance<IReadOnlyConfiguration>();
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
			// TODO: Add insite RSS to #alle-i-dips
			// TODO: Add support for creating bugs from within Slack: @DIPS-bot CreateBug Shit doesnt work! | helt-ainnsles | 17.2
			try
            {
                if (MessageIsInvalid(message))
                {
                    return;
                }

                message.Text = message.Text.ToLower();
                var messageHandler = GetMessageHandler(message);
                await messageHandler.HandleMessage(message);
            }
            catch (Exception exception)
            {
                await slackIntegration.SendDirectMessage(adminUser, "I died:\n" + exception);
                fatalExceptionHandler(exception);
            }
        }

        MessageHandler GetMessageHandler(SlackMessage message)
        {
            if (message.ChatHub.Type == SlackChatHubType.Group)
            {
#if DEBUG
                if (message.ChatHub.Name == "#bot-test")
#else
                if (message.ChatHub.Name == "#trondheim")
#endif
                {
                    return serviceContainer.GetInstance<TrondheimMessageHandler>();
                }
            }

#if !DEBUG
            if (message.ChatHub.Type == SlackChatHubType.Channel)
			{
			    return serviceContainer.GetInstance<ChannelMessageHandler>();
			}

            if (message.ChatHub.Type == SlackChatHubType.DM)
            {
                if (message.User.Id == adminUser.Id) {
                    return serviceContainer.GetInstance<AdminMessageHandler>();
                }

                return serviceContainer.GetInstance<RegularUserMessageHandler>();
            }
#else
            if (message.ChatHub.Type == SlackChatHubType.DM)
            {
                if (message.User.Id == adminUser.Id)
                {
                    return serviceContainer.GetInstance<AdminMessageHandler>();
                }
            }
#endif

            return serviceContainer.GetInstance<MessageHandler>();
        }

		static bool MessageIsInvalid(SlackMessage message) =>
	        message == null && 
            message.User == null && 
            string.IsNullOrEmpty(message.User.Id) && 
            string.IsNullOrEmpty(message.Text) && 
            message.ChatHub == null;
	}
}