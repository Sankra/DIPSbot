using System;
namespace Hjerpbakk.DIPSBot
{
    public interface IReadOnlyConfiguration
    {
		string SlackAPIToken { get;  }
		string AdminUser { get;  }
		string BotUser { get;  }
        string KitchenServiceURL { get; }	
		Action<Exception> FatalExceptionHandler { get; }
    }
}
