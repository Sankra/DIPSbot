using System;
namespace Hjerpbakk.DIPSBot
{
    public class Configuration
    {
        public string SlackAPIToken { get; set; }
        public string AdminUser { get; set;  }
        public string BotUser { get; set; }
        public string KitchenServiceURLDefault { 
            get { return KitchenServiceURL; }
            set { KitchenServiceURL = value; } 
        }
        public Action<Exception> FatalExceptionHandler { get; set; }

        // TODO: User "proper" service discovery
        public static string KitchenServiceURL { get; set; }
    }
}
