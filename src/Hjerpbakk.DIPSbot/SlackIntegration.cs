using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot.Extensions;
using Hjerpbakk.DIPSBot;
using Hjerpbakk.DIPSBot.Configuration;
using SlackConnector;
using SlackConnector.EventHandlers;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSbot
{
	/// <summary>
	///     Wrapps the Slack APIs needed for Profilebot.
	/// </summary>
	public sealed class SlackIntegration : ISlackIntegration
	{
		readonly ISlackConnector connector;
		readonly string slackKey;

		ISlackConnection connection;

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="connector">The Slack connector to use.</param>
		/// <param name="configuration">The Slack configuration.</param>
		public SlackIntegration(ISlackConnector connector, IReadOnlyAppConfiguration configuration)
		{
			this.connector = connector ?? throw new ArgumentNullException(nameof(connector));

            // TODO: sjekk configuration mot null
            if (string.IsNullOrEmpty(configuration.SlackAPIToken))
			{
                throw new ArgumentException(nameof(configuration.SlackAPIToken));
			}

            slackKey = configuration.SlackAPIToken;
		}

		/// <summary>
		///     Raised everytime the bot gets a DM.
		/// </summary>
		public event MessageReceivedEventHandler MessageReceived
		{
			add => connection.OnMessageReceived += value;
			remove => connection.OnMessageReceived -= value;
		}

		/// <summary>
		///     Connects the bot to Slack.
		/// </summary>
		/// <returns>No object or value is returned by this method when it completes.</returns>
		public async Task Connect()
		{
			connection = await connector.Connect(slackKey);
            if (connection == null) {
                throw new ArgumentException("Could not connect to Slack.");
            }
		}

        // TODO: Abonner på flere events og track feil bedre
        // TODO: Less typing notifications...

        public async Task Close()
		{
            await connection.Close();
		}

		/// <summary>
		///     Gets all the users in the Slack team.
		/// </summary>
		/// <returns>All users.</returns>
		public async Task<IEnumerable<SlackUser>> GetAllUsers()
		{
			return await connection.GetUsers();
		}

		/// <summary>
		///     Gets the user with the given Id.
		/// </summary>
		/// <param name="userId">The id of the user to be found.</param>
		/// <returns>The wanted user or null if not found.</returns>
		public async Task<SlackUser> GetUser(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentException(nameof(userId));
			}

			return connection.UserCache.ContainsKey(userId)
				? connection.UserCache[userId]
				: (await GetAllUsers()).SingleOrDefault(u => u.Id == userId);
		}

		/// <summary>
		///     Sends a DM to the given user.
		/// </summary>
		/// <param name="user">The recipient of the DM.</param>
		/// <param name="text">The message itself.</param>
		/// <returns>No object or value is returned by this method when it completes.</returns>
		public async Task SendDirectMessage(SlackUser user, string text)
		{
			user.Guard();
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentException(nameof(text));
			}

			var channel = await connection.JoinDirectMessageChannel(user.Id);
			var message = new BotMessage { ChatHub = channel, Text = text };
			await connection.Say(message);
		}

        public async Task SendMessageToChannel(SlackChatHub channel, string text, params SlackAttachment[] attachments) {
            var message = new BotMessage { ChatHub = channel, Text = text };
            if (attachments.Length > 0) {
                message.Attachments = attachments;
            }

			await connection.Say(message);
        }

		public async Task AddUsersToChannel(IEnumerable<SlackUser> users, string channelName)
		{
			// TODO: nullsjekker
			var channels = await connection.GetChannels();
			var devChannel = channels.Single(c => c.Name == channelName);
		}
	}
}