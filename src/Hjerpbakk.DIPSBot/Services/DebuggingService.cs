using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.MessageHandlers;
using Hjerpbakk.DIPSBot.Telemetry;

namespace Hjerpbakk.DIPSBot.Services
{
    class DebuggingService : IDebuggingService
    {
        readonly HttpClient httpClient;
        readonly TelemetryServiceClient telemetryClient;
        readonly string versionAndMode;

        bool debugging;

        public DebuggingService(HttpClient httpClient, TelemetryServiceClient telemetryClient)
        {
            this.httpClient = httpClient;
            this.telemetryClient = telemetryClient;

            CheckIfDEBUG();
            RunningInDebugMode = debugging;
            versionAndMode = $"{Assembly.GetExecutingAssembly().GetName().Version} {(RunningInDebugMode ? "DEBUG" : "RELEASE")}";
        }

        public bool RunningInDebugMode { get; }

        [Conditional("DEBUG")]
        void CheckIfDEBUG()
        {
            debugging = true;
        }

        public string GetVersionInfo(MessageHandler activeMessageHandler)
        =>  $"{versionAndMode} in {activeMessageHandler?.GetType().Name} on {GetPublicIP().GetAwaiter().GetResult()}";


        async Task<string> GetPublicIP() {
            try
            {
                // TODO: Blog snippet: get public ip of running machine c#   
                var ipHTML = await httpClient.GetStringAsync("http://checkip.dyndns.org/");
                int first = ipHTML.IndexOf("Address: ", StringComparison.CurrentCulture) + 9;
                int last = ipHTML.LastIndexOf("</body>", StringComparison.CurrentCulture);
                var ip = ipHTML.Substring(first, last - first);
                return ip;
            }
            catch (Exception e)
            {
                telemetryClient.TrackException(e);
                return "No IP found";
            }
        }
    }
}
