using System;
using System.IO;
using Hjerpbakk.DIPSBot;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Telemetry;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Newtonsoft.Json;

namespace Hjerpbakk.DIPSbot.Runner
{
    class Program
	{
        static void Main()
		{
			try
            {
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

				var res = dipsBot.Start(configuration).GetAwaiter().GetResult();
				if (!string.IsNullOrEmpty(res))
				{
					Console.WriteLine(res);
					Environment.Exit(1);
				}
            }
            catch (Exception e)
			{
				Console.WriteLine("Could not start DIPSbot.");
                Console.WriteLine(e);
			}
		}

        static AppConfiguration ReadConfig() {
            return JsonConvert.DeserializeObject<AppConfiguration>(File.ReadAllText("config.json"));
        }
	}
}