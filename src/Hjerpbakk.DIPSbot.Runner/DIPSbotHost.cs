using LightInject;
using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot.Services;
using SlackConnector;
using Hjerpbakk.DIPSBot;
using System.Threading;
using System.Net.Http;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Actions;

namespace Hjerpbakk.DIPSbot.Runner
{
    class DIPSbotHost
	{
        static readonly ManualResetEvent manualResetEvent;

		DIPSbotImplementation DIPSbot;

        static DIPSbotHost() {
            manualResetEvent = new ManualResetEvent(false);
        }

        public async Task<string> Start(Configuration configuration)
		{
            try
            {
				configuration.FatalExceptionHandler = RestartBot;
				while (true)
				{
                    Console.WriteLine("Starting DIPSbot...");
					var serviceContainer = CompositionRoot(configuration);
					DIPSbot = serviceContainer.GetInstance<DIPSbotImplementation>();
					try
					{
						await DIPSbot.Connect();
					}
					catch (Exception e)
					{
						Console.WriteLine($"Error connecting to Slack: {e}");
						return "";
					}

                    Console.WriteLine("DIPSbot started.");
					manualResetEvent.WaitOne();

					Console.WriteLine("Stopping before restart...");
					var res = Stop().GetAwaiter().GetResult();
					serviceContainer.Dispose();

					manualResetEvent.Reset();
				}
			}
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public async Task<string> Stop()
		{
            try {
				Console.WriteLine("Disconnecting...");
				await DIPSbot.Close();
				DIPSbot = null;
                Console.WriteLine("DIPSbot stopped.");
            } catch (Exception e) {
                return e.ToString();
            }

            return "";
		}

        void RestartBot(Exception exception) {
            manualResetEvent.Set();
            Console.WriteLine("DIPSBot crashed:");
            Console.WriteLine(exception);
        }

		static IServiceContainer CompositionRoot(Configuration configuration)
		{
			var serviceContainer = new ServiceContainer();

            serviceContainer.RegisterInstance(configuration);
            serviceContainer.RegisterInstance(new HttpClient());
			serviceContainer.Register<ISlackConnector, SlackConnector.SlackConnector>(new PerContainerLifetime());
			serviceContainer.Register<ISlackIntegration, SlackIntegration>(new PerContainerLifetime());

            serviceContainer.Register<IKitchenResponsibleClient, KitchenResponsibleClient>(new PerContainerLifetime());
			serviceContainer.Register<IOrganizationService, FileOrganizationService>(new PerContainerLifetime());

            serviceContainer.Register<UserKitchenResponsibleAction>();
            serviceContainer.Register<TrondheimKitchenResponsibleAction>();

			serviceContainer.Register<DIPSbotImplementation>();

			return serviceContainer;
		}
	}
}