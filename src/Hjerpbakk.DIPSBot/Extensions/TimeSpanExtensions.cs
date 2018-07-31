using System;

namespace Hjerpbakk.DIPSBot.Extensions {
    public static class TimeSpanExtensions {
        public static string FormatWithoutSeconds(this TimeSpan timeSpan) {
            var days = (int)Math.Floor(timeSpan.TotalDays);
            var hours = (int)Math.Floor(timeSpan.TotalHours);
            var minutes = timeSpan.Minutes + (timeSpan.Seconds > 0 ? 1 : 0);
            if (hours == 0) {
                return Minutes();
            }

            if (days == 0) {
                if (minutes == 0) {
                    return Hours();
                }

                return Hours() + " " + Minutes();
            }

            hours = timeSpan.Hours + (timeSpan.Minutes > 0 ? 1 : 0);
            return Days() + " " + Hours();

            string Days() => days + " day" + SingularOrPlural(days);
            string Hours() => hours + " hour" + SingularOrPlural(hours);
            string Minutes() => minutes + " min" + SingularOrPlural(minutes);
            string SingularOrPlural(int value) => value == 1 ? "" : "s";
        }
    }
}
