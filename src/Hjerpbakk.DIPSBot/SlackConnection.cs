using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Configuration;
using SlackConnector;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot
{
    public class SlackConnection
    {
        readonly string slackKey;
        readonly ISlackConnector connector;

        ISlackConnection connection;
        Func<SlackMessage, Task> messageReceived;

        public SlackConnection(ISlackConnector connector, IReadOnlyAppConfiguration configuration)
        {
            this.connector = connector ?? throw new ArgumentNullException(nameof(connector));
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (string.IsNullOrEmpty(configuration.SlackAPIToken))
            {
                throw new ArgumentException(nameof(configuration.SlackAPIToken));
            }

            slackKey = configuration.SlackAPIToken;
        }

        /// <summary>
        ///     Connects the bot to Slack.
        /// </summary>
        /// <returns>No object or value is returned by this method when it completes.</returns>
        public async Task Connect(Func<SlackMessage, Task> messageReceived)
        {
            connection = await connector.Connect(slackKey);
            if (connection == null)
            {
                throw new ArgumentException("Could not connect to Slack.");
            }

            this.messageReceived = messageReceived;
            connection.OnMessageReceived += Connection_OnMessageReceived;
        }

        public async Task<ISlackConnection> GetConnection() {
            if (connection.IsConnected) {
                return connection;
            }

            await Close();
            await Connect(messageReceived):
            return connection;
        }

        // TODO: Abonner på flere events og track feil bedre?

        public async Task Close()
        {
            connection.OnMessageReceived -= Connection_OnMessageReceived;
            await connection.Close();
        }

        public async Task<SlackChatHub> GetDirectMessageChannel(string userId) =>
            await (await GetConnection()).JoinDirectMessageChannel(userId);

        public async Task SendMessage(BotMessage message) {
            await (await GetConnection()).Say(message);
        }

        public async Task<IEnumerable<SlackUser>> GetAllUsers() 
            => await (await GetConnection()).GetUsers();

        public async Task<SlackUser> GetUser(string userId) =>
            (await GetConnection()).UserCache.ContainsKey(userId)
                ? (await GetConnection()).UserCache[userId]
                : (await GetAllUsers()).SingleOrDefault(u => u.Id == userId);

        async Task Connection_OnMessageReceived(SlackMessage message)
        {
            await messageReceived(message);
        }
    }
}
