using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot
{
    // TODO: Felles interface for connect og close og sånn?
    public class QueuedSlackConnection
    {
        readonly SlackConnection slackConnection;

        readonly Thread postingThread;
        readonly ConcurrentQueue<BotMessage> slackMessageQueue;
        readonly TimeSpan messageLoopSleepTime;

        public QueuedSlackConnection(SlackConnection slackConnection)
        {
            this.slackConnection = slackConnection;

            // TODO: perhaps adjust
            messageLoopSleepTime = TimeSpan.FromSeconds(1D);
            slackMessageQueue = new ConcurrentQueue<BotMessage>();
            var threadStart = new ThreadStart(Run);
            postingThread = new Thread(threadStart);
        }

        public async Task Connect(Func<SlackMessage, Task> messageReceived) {
            await slackConnection.Connect(messageReceived);
            postingThread.Start();
        }
            
        public async Task Close() => await slackConnection.Close();

        // TODO: Feilhåndtering må funke anderledes nå som vi kan ha meldinger igjen på køen...
#pragma warning disable RECS0135 // Function does not reach its end or a 'return' statement by any of possible execution paths
        public void Run()
        {
            while (true) {
                var loopStartTimeStamp = DateTime.UtcNow;
                if (slackMessageQueue.TryDequeue(out BotMessage message)) {
                    try
                    {
                        slackConnection.GetConnection().GetAwaiter().GetResult().Say(message).GetAwaiter();
                    }
                    catch (Exception ex)
                    {
                        // TODO: Si fra at har krasjet uten å ta ned hele dritten
                    }

                }

                var waitTime = messageLoopSleepTime - (DateTime.UtcNow - loopStartTimeStamp);
                waitTime = waitTime.Milliseconds < 1 ? TimeSpan.FromMilliseconds(1) : waitTime;
                Thread.Sleep(waitTime.Milliseconds);
            }
        }
#pragma warning restore RECS0135 // Function does not reach its end or a 'return' statement by any of possible execution paths

        public void SendMessage(BotMessage message) {
            slackMessageQueue.Enqueue(message);
        }

        public async Task<SlackChatHub> GetDirectMessageChannel(string userId) => 
            await slackConnection.GetDirectMessageChannel(userId);

        public async Task<SlackUser> GetUser(string userId) =>
            await slackConnection.GetUser(userId);

        public async Task<IEnumerable<SlackUser>> GetAllUsers() =>
            await slackConnection.GetAllUsers();
    }
}
