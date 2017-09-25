using System;
using System.Globalization;
using Hjerpbakk.DIPSBot.Clients;
using Newtonsoft.Json;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Model
{
    public struct EmployeeWeek
    {
        public ushort WeekNumber { get; set; }
        [JsonConverter(typeof(SlackUserJsonConverter))]
        public SlackUser SlackUser { get; set; }

        public string FormattedEmployeeWeek => WeekNumber + ". " + SlackUser.FormattedUserId;

		public static ushort GetIso8601WeekOfYear(DateTime time)
		{
			// Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
			// be the same week# as whatever Thursday, Friday or Saturday are,
			// and we always get those right
			DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
			if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
			{
				time = time.AddDays(3);
			}

			// Return the week of our adjusted day
			return (ushort)CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
		}

		public static ushort GetNextWeek(ushort week) =>
			week == 52 ? (ushort)1 : (ushort)(week + 1);

		public static ushort GetPreviousWeek(ushort week) =>
			week == 1 ? (ushort)52 : (ushort)(week - 1);
    }
}
