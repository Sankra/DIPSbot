using System;
using System.Runtime.CompilerServices;

namespace Hjerpbakk.DIPSBot.Telemetry
{
    public interface ITelemetryServiceClient
    {
        void TrackException(Exception exception, string additionalInfo = null, [CallerMemberName] string callsite = null);
        void StartMetric(string name);
        void EndMetric(string name, string metricName = null);
    }
}
