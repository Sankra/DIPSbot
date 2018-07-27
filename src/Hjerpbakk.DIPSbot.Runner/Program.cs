using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Telemetry;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Newtonsoft.Json;

namespace Hjerpbakk.DIPSbot.Runner {
    class Program {
        static async Task Main() {
            try {
                Console.WriteLine("Fetching configuration...");
                var configuration = ReadConfig();
                configuration.Context = new Context();

                ITelemetryServiceClient telemetryServiceClient;
                if (string.IsNullOrEmpty(configuration.InstrumentationKey)) {
                    telemetryServiceClient = new EmptyTelemetryServiceClient();
                } else {
                    TelemetryConfiguration.Active.InstrumentationKey = configuration.InstrumentationKey;
                    var telemetryClient = new TelemetryClient();
                    telemetryClient.Context.Component.Version = configuration.Context.Version;
                    telemetryServiceClient = new TelemetryServiceClient(telemetryClient);
                }

                var dipsBot = new DIPSbotHost(telemetryServiceClient);
                await dipsBot.Start(configuration);
            } catch (Exception e) {
                Console.WriteLine("Could not start DIPSbot.");
                Console.WriteLine(e.Demystify());
                throw;
            }
        }

        static AppConfiguration ReadConfig() {
            return JsonConvert.DeserializeObject<AppConfiguration>(File.ReadAllText("config.json"));
        }
    }
}