using System;
namespace Hjerpbakk.DIPSBot.Configuration
{
    public interface IReadOnlyAppConfiguration
    {
		string SlackAPIToken { get;  }
		string AdminUser { get;  }
		string BotUser { get;  }
        string KitchenServiceURL { get; }	
		Action<Exception> FatalExceptionHandler { get; }
    }
}
