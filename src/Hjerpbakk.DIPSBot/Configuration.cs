using System;
namespace Hjerpbakk.DIPSBot
{
    public class Configuration
    {
        public string SlackAPIToken { get; set; }
        public Action<Exception> FatalExceptionHandler { get; set; }
    }
}
