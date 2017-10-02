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
using Hjerpbakk.DIPSBot.MessageHandlers;

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
                    IServiceContainer serviceContainer;
					try
					{
						serviceContainer = await CompositionRoot(configuration);
						DIPSbot = serviceContainer.GetInstance<DIPSbotImplementation>();
						await DIPSbot.Connect();
					}
					catch (Exception e)
					{
						Console.WriteLine($"Error connecting to Slack or other services: {e}");
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

		static async Task<IServiceContainer> CompositionRoot(Configuration configuration)
		{
            var serviceDiscoveryClient = new ServiceDiscoveryClient(configuration);
            await serviceDiscoveryClient.SetKitchenServiceURL();
                                        
			var serviceContainer = new ServiceContainer();
            serviceContainer.RegisterInstance<IServiceContainer>(serviceContainer);

            serviceContainer.RegisterInstance(configuration);
            serviceContainer.RegisterInstance<IReadOnlyConfiguration>(configuration);
            serviceContainer.RegisterInstance(serviceDiscoveryClient);
            serviceContainer.RegisterInstance(new HttpClient());
			serviceContainer.Register<ISlackConnector, SlackConnector.SlackConnector>(new PerContainerLifetime());
			serviceContainer.Register<ISlackIntegration, SlackIntegration>(new PerContainerLifetime());

            serviceContainer.Register<IKitchenResponsibleClient, KitchenResponsibleClientWithURLPolling>(new PerContainerLifetime());
			serviceContainer.Register<IOrganizationService, FileOrganizationService>(new PerContainerLifetime());

            serviceContainer.Register<AdminMessageHandler>(new PerContainerLifetime());
            serviceContainer.Register<ChannelMessageHandler>(new PerContainerLifetime());
            serviceContainer.Register<MessageHandler>(new PerContainerLifetime());
            serviceContainer.Register<RegularUserMessageHandler>(new PerContainerLifetime());
            serviceContainer.Register<TrondheimMessageHandler>(new PerContainerLifetime());

            serviceContainer.Register<AddDevelopersToUtviklingChannelAction>(new PerContainerLifetime());
            serviceContainer.Register<KitchenResponsibleAction>(new PerContainerLifetime());
            serviceContainer.Register<AddEmployeeAction>(new PerContainerLifetime());
            serviceContainer.Register<ThanksAction>(new PerContainerLifetime());
            serviceContainer.Register<NegativeAction>(new PerContainerLifetime());
            serviceContainer.Register<WeekAction>(new PerContainerLifetime());
            serviceContainer.Register<RemoveEmployeeAction>(new PerContainerLifetime());
            serviceContainer.Register<KitchenServiceIPAction>(new PerContainerLifetime());
			
			serviceContainer.Register<DIPSbotImplementation>(new PerContainerLifetime());

			return serviceContainer;
		}
	}
}