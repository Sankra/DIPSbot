﻿using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.MessageHandlers;
using Hjerpbakk.DIPSBot.Services;
using Hjerpbakk.DIPSBot.Telemetry;
using Hjerpbakk.ServiceDiscovery.Client;
using Hjerpbakk.ServiceDiscovery.Client.Model;
using LightInject;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSbot
{
    class DIPSbotImplementation
    {
        readonly IServiceContainer serviceContainer;
        readonly ISlackIntegration slackIntegration;
        readonly IDebuggingService debuggingService;
        readonly ITelemetryServiceClient telemetryServiceClient;
        readonly ServiceDiscoveryClient serviceDiscoveryClient;

        readonly Action<Exception> fatalExceptionHandler;
        readonly SlackUser adminUser;
        readonly Service service;

        public DIPSbotImplementation(IServiceContainer serviceContainer)
        {
            this.serviceContainer = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));

            slackIntegration = serviceContainer.GetInstance<ISlackIntegration>();
            var configuration = serviceContainer.GetInstance<IReadOnlyAppConfiguration>();
            fatalExceptionHandler = configuration.FatalExceptionHandler;
            adminUser = new SlackUser { Id = configuration.AdminUser };
            debuggingService = serviceContainer.GetInstance<IDebuggingService>();
            telemetryServiceClient = serviceContainer.GetInstance<ITelemetryServiceClient>();
            serviceDiscoveryClient = serviceContainer.GetInstance<ServiceDiscoveryClient>();
            service = configuration.Service;
        }

        /// <summary>
        ///     Disconnects the bot from Slack.
        /// </summary>
        public async Task Close()
        {
            await slackIntegration.Close();
        }

        /// <summary>
        ///     Connects the bot to Slack.
        /// </summary>
        /// <returns>No object or value is returned by this method when it completes.</returns>
        public async Task Connect()
        {
            await slackIntegration.Connect(MessageReceived);
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
                await serviceDiscoveryClient.Heartbeat(service);
                if (MessageIsInvalid(message))
                {
                    return;
                }

                message.Text = message.Text.Trim().ToLower();
                messageHandler = debuggingService.RunningInDebugMode ?
                                         GetDEBUGMessageHandler(message) :
                                         GetMessageHandler(message);

                if (!(messageHandler is NoopMessageHandler)) {
                    const string TimeSpentOnHandledMessage = "TimeSpentOnHandledMessage";
                    telemetryServiceClient.StartMetric(TimeSpentOnHandledMessage);

                    var actionName = await messageHandler.HandleMessage(message);

                    // Fuck it for now if message fails
                    telemetryServiceClient.EndMetric(TimeSpentOnHandledMessage, actionName);
                }

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

            return serviceContainer.GetInstance<NoopMessageHandler>();
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

            return serviceContainer.GetInstance<NoopMessageHandler>();
        }

		static bool MessageIsInvalid(SlackMessage message) =>
	        message == null || 
            message.User == null || 
            string.IsNullOrEmpty(message.User.Id) || 
            string.IsNullOrEmpty(message.Text) || 
            message.ChatHub == null;
	}
}