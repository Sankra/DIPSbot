using System;
namespace Hjerpbakk.DIPSBot.Configuration
{
    public interface IReadOnlyAppConfiguration
    {
		string SlackAPIToken { get;  }
		string AdminUser { get;  }
		string BotUser { get;  }

        string KitchenServiceURL { get; }
        string ComicsServiceURL { get; set; }

		Action<Exception> FatalExceptionHandler { get; }
    }
}
