using System;
using System.Runtime.CompilerServices;

namespace Hjerpbakk.DIPSBot.Telemetry
{
    public class EmptyTelemetryServiceClient : ITelemetryServiceClient
    {
        public void EndMetric(string name, string metricName = null)
        {
        }

        public void StartMetric(string name)
        {
        }

        public void TrackException(Exception exception, string additionalInfo = null, [CallerMemberName] string callsite = null)
        {
        }
    }
}
