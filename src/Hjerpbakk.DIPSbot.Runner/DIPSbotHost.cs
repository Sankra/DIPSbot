using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot.Services;
using SlackConnector;
using Hjerpbakk.DIPSBot.Runner;

namespace Hjerpbakk.DIPSbot.Runner
{
	class DIPSbotHost
	{
		DIPSbotImplementation DIPSbot;

        public void Start(Configuration configuration)
		{
			var serviceContainer = CompositionRoot(configuration);
			DIPSbot = serviceContainer.GetInstance<DIPSbotImplementation>();
			DIPSbot
				.Connect()
				.ContinueWith(task => {
					if (!task.IsCompleted || task.IsFaulted)
					{
						Console.WriteLine($"Error connecting to Slack: {task.Exception}");
                    // TODO: Ikke restart her...
					}
				});
             
			// TODO: Må restarte dersom krasjer eller blir disconnected
            // TODO: Ta ned hele containeren mellom hver kjøring
        }

        public async Task<string> Stop()
		{
            try {
				Console.WriteLine("Disconnecting...");
				await DIPSbot.Close();
				DIPSbot = null;
            } catch (Exception e) {
                return e.ToString();
            }

            return "";
		}

		static IServiceContainer CompositionRoot(Configuration configuration)
		{
			var serviceContainer = new ServiceContainer();

            serviceContainer.RegisterInstance(configuration.SlackAPIToken);
			serviceContainer.Register<ISlackConnector, SlackConnector.SlackConnector>(new PerContainerLifetime());
			serviceContainer.Register<ISlackIntegration, SlackIntegration>(new PerContainerLifetime());

			serviceContainer.Register<IOrganizationService, FileOrganizationService>(new PerContainerLifetime());

			serviceContainer.Register<DIPSbotImplementation>();

			return serviceContainer;
		}
	}
}