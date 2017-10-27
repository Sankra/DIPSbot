using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.MessageHandlers;
using Hjerpbakk.DIPSBot.Services;
using LightInject;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSbot
{
    class DIPSbotImplementation
    {
        readonly IServiceContainer serviceContainer;
        readonly ISlackIntegration slackIntegration;
        readonly IDebuggingService debuggingService;

        readonly Action<Exception> fatalExceptionHandler;
        readonly SlackUser adminUser;

        public DIPSbotImplementation(IServiceContainer serviceContainer)
        {
            this.serviceContainer = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));

            slackIntegration = serviceContainer.GetInstance<ISlackIntegration>();
            var configuration = serviceContainer.GetInstance<IReadOnlyAppConfiguration>();
            fatalExceptionHandler = configuration.FatalExceptionHandler;
            adminUser = new SlackUser { Id = configuration.AdminUser };
            debuggingService = serviceContainer.GetInstance<IDebuggingService>();
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
            // TODO: Add support for creating bugs from within Slack: @DIPS-bot CreateBug Shit doesnt work! | helt-ainnsles | 17.2
            MessageHandler messageHandler = null;
			try
            {
                if (MessageIsInvalid(message))
                {
                    return;
                }

                message.Text = message.Text.Trim().ToLower();
                messageHandler = debuggingService.RunningInDebugMode ?
                                         GetDEBUGMessageHandler(message) :
                                         GetMessageHandler(message);
                
                await messageHandler.HandleMessage(message);
            }
            catch (Exception exception)
            {
                await slackIntegration.SendDirectMessage(adminUser, $"I died {debuggingService.GetVersionInfo(messageHandler)}:\n{exception}");
                fatalExceptionHandler(exception);
            }
        }

        MessageHandler GetMessageHandler(SlackMessage message)
        {
            if (message.ChatHub.Type == SlackChatHubType.Group)
            {
                if (message.ChatHub.Name == "#trondheim")
                {
                    return serviceContainer.GetInstance<TrondheimMessageHandler>();
                }

                return serviceContainer.GetInstance<ChannelMessageHandler>();
            }

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

            return serviceContainer.GetInstance<MessageHandler>();
        }

        MessageHandler GetDEBUGMessageHandler(SlackMessage message) {
            if (message.ChatHub.Type == SlackChatHubType.Group)
            {
                if (message.ChatHub.Name == "#bot-test")
                {
                    return serviceContainer.GetInstance<TrondheimMessageHandler>();
                }
            }

            if (message.ChatHub.Type == SlackChatHubType.DM)
            {
                if (message.User.Id == adminUser.Id)
                {
                    return serviceContainer.GetInstance<AdminMessageHandler>();
                }
            }

            return serviceContainer.GetInstance<MessageHandler>();
        }

		static bool MessageIsInvalid(SlackMessage message) =>
	        message == null || 
            message.User == null || 
            string.IsNullOrEmpty(message.User.Id) || 
            string.IsNullOrEmpty(message.Text) || 
            message.ChatHub == null;
	}
}