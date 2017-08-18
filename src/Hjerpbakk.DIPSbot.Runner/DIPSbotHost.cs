using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hjerpbakk.DIPSbot.Services;
using SlackConnector;

namespace Hjerpbakk.DIPSbot.Runner {
    class DIPSbotHost {
        DIPSbotImplementation DIPSbot;

        public void Start() {
            var serviceContainer = CompositionRoot();
            DIPSbot = serviceContainer.GetInstance<DIPSbotImplementation>();
            DIPSbot
                .Connect()
                .ContinueWith(task => {
                    if (!task.IsCompleted || task.IsFaulted) {
                        Console.WriteLine($"Error connecting to Slack: {task.Exception}");
                    }
                });
        }

        public void Stop() {
            Console.WriteLine("Disconnecting...");
            DIPSbot.Dispose();
            DIPSbot = null;
        }

        static IServiceContainer CompositionRoot() {
            var serviceContainer = new ServiceContainer();

            serviceContainer.RegisterInstance("xoxb-74447052882-CjgPJUCBXfPydIZ9dMD5V3Ey");
            serviceContainer.Register<ISlackConnector, SlackConnector.SlackConnector>(new PerContainerLifetime());
            serviceContainer.Register<ISlackIntegration, SlackIntegration>(new PerContainerLifetime());

            serviceContainer.Register<IOrganizationService, FileOrganizationService>(new PerContainerLifetime());

            serviceContainer.Register<DIPSbotImplementation>();

            return serviceContainer;
        }
    }
}