using System.Reflection;

namespace Hjerpbakk.DIPSBot.Telemetry
{
    public class Context
    {
        public Context()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public string Version { get; }
    }
}
