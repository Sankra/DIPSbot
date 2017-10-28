using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Hjerpbakk.DIPSBot.Telemetry
{
    public class TelemetryServiceClient
    {
        readonly TelemetryClient telemetryClient;

        readonly ConcurrentDictionary<string, DateTime> activeMetrics;

        public TelemetryServiceClient(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
            activeMetrics = new ConcurrentDictionary<string, DateTime>();
        }

        public void TrackException(Exception exception, string additionalInfo = null, [CallerMemberName] string callsite = null) {
            if (exception == null) {
                return;
            }

            var telemetry = new ExceptionTelemetry(exception)
            {
                Message = callsite,
                Timestamp = DateTimeOffset.UtcNow
            };

            if (additionalInfo != null) {
                telemetry.Message = telemetry.Message + additionalInfo;
            } 

            telemetryClient.TrackException(telemetry);
        }

        // TODO: Use using using :P
        public void StartMetric(string name) {
            activeMetrics.AddOrUpdate(name, DateTime.UtcNow, (a,b) => DateTime.UtcNow);
        }

        public void EndMetric(string name, string metricName = null) {
            activeMetrics.TryRemove(name, out DateTime metricStart);
            if (metricStart == DateTime.MinValue) {
                return;
            }

            var timeSpent = DateTime.UtcNow - metricStart;
            var metricValue = timeSpent.Milliseconds;
            var metric = new MetricTelemetry(metricName ?? name, 1, metricValue, metricValue, metricValue, 0D)
            {
                Timestamp = DateTime.UtcNow
            };
            telemetryClient.TrackMetric(metric);
        }
    }
}
