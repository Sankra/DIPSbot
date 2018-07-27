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
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.ServiceDiscovery.Client;
using Hjerpbakk.DIPSBot.Services;
using Hjerpbakk.DIPSBot.Telemetry;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace Hjerpbakk.DIPSbot.Runner {
    class DIPSbotHost {
        static readonly ManualResetEvent manualResetEvent;

        readonly ITelemetryServiceClient telemetryServiceClient;

        int restartCount;
        DIPSbotImplementation DIPSbot;

        static DIPSbotHost() {
            manualResetEvent = new ManualResetEvent(false);
        }

        public DIPSbotHost(ITelemetryServiceClient telemetryServiceClient) {
            this.telemetryServiceClient = telemetryServiceClient;
        }

        public async Task<string> Start(AppConfiguration configuration) {
            try {
                configuration.FatalExceptionHandler = RestartBot;
                while (true) {
                    Console.WriteLine("Starting DIPSbot...");
                    IServiceContainer serviceContainer;
                    try {
                        serviceContainer = await CompositionRoot(configuration);
                        DIPSbot = serviceContainer.GetInstance<DIPSbotImplementation>();
                        await DIPSbot.Connect();
                    } catch (Exception e) {
                        e = e.Demystify();
                        Console.WriteLine($"Error connecting to Slack or other services: {e}");
                        telemetryServiceClient.TrackException(e, $"Error connecting to Slack {restartCount}");
                        return "";
                    }

                    Console.WriteLine("DIPSbot started.");
                    manualResetEvent.WaitOne();

                    Console.WriteLine("Stopping before restart...");
                    var res = Stop().GetAwaiter().GetResult();
                    serviceContainer.Dispose();

                    manualResetEvent.Reset();
                }
            } catch (Exception e) {
                e = e.Demystify();
                telemetryServiceClient.TrackException(e, $"Died unexpectedly {restartCount}");
                return e.ToString();
            }
        }

        public async Task<string> Stop() {
            try {
                Console.WriteLine("Disconnecting...");
                await DIPSbot.Close();
                DIPSbot = null;
                Console.WriteLine("DIPSbot stopped.");
            } catch (Exception e) {
                return e.Demystify().ToString();
            }

            return "";
        }

        void RestartBot(Exception exception) {
            Interlocked.Increment(ref restartCount);
            manualResetEvent.Set();
            Console.WriteLine("Trying to restart. Cause of death:");
            exception = exception.Demystify();
            Console.WriteLine(exception);

            telemetryServiceClient.TrackException(exception, $"Bot died during message handling, trying to restart {restartCount}");
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
            serviceContainer.Register<TrondheimBysykkelClient>(new PerContainerLifetime());

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