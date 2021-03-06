﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlackConnector.EventHandlers;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSbot
{
	/// <summary>
	///     Interface for the Slack APIs needed for Profilebot.
	/// </summary>
	public interface ISlackIntegration
	{
        Task Connect(Func<SlackMessage, Task> messageReceived);

        Task Close();

		/// <summary>
		///     Gets all the users in the Slack team.
		/// </summary>
		/// <returns>All users.</returns>
		Task<IEnumerable<SlackUser>> GetAllUsers();

		/// <summary>
		///     Gets the user with the given Id.
		/// </summary>
		/// <param name="userId">The id of the user to be found.</param>
		/// <returns>The wanted user or null if not found.</returns>
		Task<SlackUser> GetUser(string userId);

		/// <summary>
		///     Sends a DM to the given user.
		/// </summary>
		/// <param name="user">The recipient of the DM.</param>
		/// <param name="text">The message itself.</param>
		/// <returns>No object or value is returned by this method when it completes.</returns>
		Task SendDirectMessage(SlackUser user, string text);

        Task SendMessageToChannel(SlackChatHub channel, string text, params SlackAttachment[] attachments);
	}
}