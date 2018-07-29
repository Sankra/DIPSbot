using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using BikeshareClient;
using Hjerpbakk.DIPSbot.Services;
using Hjerpbakk.DIPSBot;
using Hjerpbakk.DIPSBot.Actions;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.MessageHandlers;
using Hjerpbakk.DIPSBot.Services;
using Hjerpbakk.DIPSBot.Telemetry;
using Hjerpbakk.ServiceDiscovery.Client;
using LightInject;
using Microsoft.Extensions.Caching.Memory;
using SlackConnector;

namespace Hjerpbakk.DIPSbot.Runner {
    class DIPSbotHost {
        readonly ITelemetryServiceClient telemetryServiceClient;

        DIPSbotImplementation DIPSbot;

        public DIPSbotHost(ITelemetryServiceClient telemetryServiceClient) {
            this.telemetryServiceClient = telemetryServiceClient;
        }

        public async Task Start(AppConfiguration configuration) {
            Console.WriteLine("Starting DIPSbot...");

            try {
                var serviceContainer = await CompositionRoot(configuration);
                DIPSbot = serviceContainer.GetInstance<DIPSbotImplementation>();
                await DIPSbot.Connect();
            } catch (Exception e) {
                e = e.Demystify();
                Console.WriteLine($"Error connecting to Slack or other services: {e}");
                telemetryServiceClient.TrackException(e, $"Error connecting to Slack or other services");
                throw;
            }

            Console.WriteLine("DIPSbot started.");
        }

        async Task<IServiceContainer> CompositionRoot(AppConfiguration configuration) {
            var serviceContainer = new ServiceContainer();
            serviceContainer.RegisterInstance<IServiceContainer>(serviceContainer);

            serviceContainer.RegisterInstance<ITelemetryServiceClient>(telemetryServiceClient);

            var httpClient = new HttpClient();
            serviceContainer.RegisterInstance(httpClient);
            var serviceDiscoveryClient = new ServiceDiscoveryClient(httpClient, configuration.ServiceDiscoveryUrl, configuration.ApiKey);

            try {
                // TODO: Kan gjøres samtidig og med bedre feilhåndtering
                var kitchenServiceTask = serviceDiscoveryClient.GetServiceURL(configuration.KitchenResponsibleServiceName);
                var comicsServiceTask = serviceDiscoveryClient.GetServiceURL(configuration.ComicsServiceName);
                await Task.WhenAll(kitchenServiceTask, comicsServiceTask);
                configuration.KitchenServiceURL = kitchenServiceTask.Result;
                configuration.ComicsServiceURL = comicsServiceTask.Result;
            } catch (Exception e) {
                // TODO: Hvordan skal actions takle at ting er utilgjengelige?
                configuration.KitchenServiceURL = null;
                configuration.ComicsServiceURL = null;
                telemetryServiceClient.TrackException(e.Demystify(), $"Could not get URLs of dependant services");
            }

            serviceContainer.RegisterInstance(configuration.Context);
            serviceContainer.RegisterInstance<IReadOnlyAppConfiguration>(configuration);
            serviceContainer.RegisterInstance<IGoogleMapsConfiguration>(configuration);
            serviceContainer.RegisterInstance<IImgurConfiguration>(configuration);
            serviceContainer.RegisterInstance(serviceDiscoveryClient);

            serviceContainer.Register<ISlackConnector, SlackConnector.SlackConnector>(new PerContainerLifetime());
            serviceContainer.Register<ISlackIntegration, SlackIntegration>(new PerContainerLifetime());
            serviceContainer.Register<SlackConnection>(new PerContainerLifetime());
            serviceContainer.Register<QueuedSlackConnection>(new PerContainerLifetime());

            serviceContainer.Register<ComicsClient>(new PerContainerLifetime());
            serviceContainer.Register<IKitchenResponsibleClient, KitchenResponsibleClient>(new PerContainerLifetime());
            serviceContainer.Register<IOrganizationService, FileOrganizationService>(new PerContainerLifetime());
            serviceContainer.Register<IDebuggingService, DebuggingService>(new PerContainerLifetime());

            serviceContainer.Register<IMemoryCache>(serviceFactory => new MemoryCache(new MemoryCacheOptions()), new PerContainerLifetime());
            serviceContainer.Register<IBikeshareClient>(serviceFactory => new Client(configuration.BikeShareApiEndpoint), new PerContainerLifetime());
            serviceContainer.Register<BikeShareClient>(new PerContainerLifetime());
            serviceContainer.Register<GoogleMapsClient>(new PerContainerLifetime());
            serviceContainer.Register<ImgurClient>(new PerContainerLifetime());

            serviceContainer.Register<AdminMessageHandler>(new PerContainerLifetime());
            serviceContainer.Register<ChannelMessageHandler>(new PerContainerLifetime());
            serviceContainer.Register<NoopMessageHandler>(new PerContainerLifetime());
            serviceContainer.Register<RegularUserMessageHandler>(new PerContainerLifetime());
            serviceContainer.Register<TrondheimMessageHandler>(new PerContainerLifetime());

            // TODO: Smoothify
            serviceContainer.Register<AddDevelopersToUtviklingChannelAction>(new PerContainerLifetime());
            serviceContainer.Register<KitchenResponsibleAction>(new PerContainerLifetime());
            serviceContainer.Register<AddEmployeeAction>(new PerContainerLifetime());
            serviceContainer.Register<ThanksAction>(new PerContainerLifetime());
            serviceContainer.Register<NegativeAction>(new PerContainerLifetime());
            serviceContainer.Register<WeekAction>(new PerContainerLifetime());
            serviceContainer.Register<RemoveEmployeeAction>(new PerContainerLifetime());
            serviceContainer.Register<ComicsAction>(new PerContainerLifetime());
            serviceContainer.Register<VersionAction>(new PerContainerLifetime());
            serviceContainer.Register<ScooterAction>(new PerContainerLifetime());
            serviceContainer.Register<SwapKitchenWeekAction>(new PerContainerLifetime());
            serviceContainer.Register<SwapAnswerAction>(new PerContainerLifetime());
            serviceContainer.Register<BikeShareAction>(new PerContainerLifetime());

            serviceContainer.Register<DIPSbotImplementation>(new PerContainerLifetime());

            return serviceContainer;
        }
    }
}