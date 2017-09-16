using System;
using System.IO;
using Hjerpbakk.DIPSBot;
using Newtonsoft.Json;

namespace Hjerpbakk.DIPSbot.Runner
{
    class Program
	{
        static void Main()
		{
			try
            {
				Console.WriteLine("Fetching configuration...");
				var configuration = ReadConfig();
				var dipsBot = new DIPSbotHost();

				var res = dipsBot.Start(configuration).GetAwaiter().GetResult();
				if (!string.IsNullOrEmpty(res))
				{
					Console.WriteLine(res);
					Environment.Exit(1);
				}
            }
            catch (Exception e)
			{
				Console.WriteLine("Could not start DIPSbot.");
                Console.WriteLine(e);
			}
		}

        static Configuration ReadConfig() {
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config.json"));
        }
	}
}