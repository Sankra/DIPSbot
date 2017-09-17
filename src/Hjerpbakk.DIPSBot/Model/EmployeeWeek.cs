using System;
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
    }
}
