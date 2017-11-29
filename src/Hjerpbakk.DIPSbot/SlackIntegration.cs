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
        readonly QueuedSlackConnection connection;

        public SlackIntegration(QueuedSlackConnection connection)
		{
            this.connection = connection;
		}

		/// <summary>
		///     Connects the bot to Slack.
		/// </summary>
		/// <returns>No object or value is returned by this method when it completes.</returns>
        public async Task Connect(Func<SlackMessage, Task> messageReceived)
		{
            await connection.Connect(messageReceived);
		}

        public async Task Close() {
            await connection.Close();
        }

        /// <summary>
        ///     Gets all the users in the Slack team.
        /// </summary>
        /// <returns>All users.</returns>
        public async Task<IEnumerable<SlackUser>> GetAllUsers() => 
            await connection.GetAllUsers();

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

            return await connection.GetUser(userId);
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

            var channel = await connection.GetDirectMessageChannel(user.Id);
			var message = new BotMessage { ChatHub = channel, Text = text };
            connection.SendMessage(message);
		}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task SendMessageToChannel(SlackChatHub channel, string text, params SlackAttachment[] attachments)
        {
            var message = new BotMessage { ChatHub = channel, Text = text };
            if (attachments.Length > 0) {
                message.Attachments = attachments;
            }

            connection.SendMessage(message);
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

		//public async Task AddUsersToChannel(IEnumerable<SlackUser> users, string channelName)
		//{
		//	// TODO: nullsjekker
		//	var channels = await connection.GetChannels();
		//	var devChannel = channels.Single(c => c.Name == channelName);
		//}
	}
}