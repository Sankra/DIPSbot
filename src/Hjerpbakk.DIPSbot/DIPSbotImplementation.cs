using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot.Services;
using Hjerpbakk.DIPSBot;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSbot
{
	internal class DIPSbotImplementation
	{
		readonly ISlackIntegration slackIntegration;
		readonly IOrganizationService organizationService;
        readonly Action<Exception> fatalExceptionHandler;
        readonly SlackUser adminUser;

		public DIPSbotImplementation(ISlackIntegration slackIntegration, IOrganizationService organizationService, Configuration configuration)
		{
			this.slackIntegration = slackIntegration ?? throw new ArgumentNullException(nameof(slackIntegration));
			this.organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
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
				if (message.Text != "utv")
				{
					await slackIntegration.SendDirectMessage(message.User, "Unknown command");
					return;
				}

				await AddDevelopersToDeveloperChannel(message);
            }
            catch (Exception exception)
            {
                await slackIntegration.SendDirectMessage(adminUser , "I died:\n" + exception);
                fatalExceptionHandler(exception);
            }
		}

		async Task AddDevelopersToDeveloperChannel(SlackMessage message)
		{
			await slackIntegration.IndicateTyping(message.User);
			var developers = await organizationService.GetDevelopers();
			var slackUsers = await slackIntegration.GetAllUsers();
		}
	}
}