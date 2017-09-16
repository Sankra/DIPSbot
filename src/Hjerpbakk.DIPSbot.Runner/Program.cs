using System;
using System.IO;
using Hjerpbakk.DIPSBot.Runner;
using Newtonsoft.Json;

namespace Hjerpbakk.DIPSbot.Runner
{
    class Program
	{
        static void Main()
		{
			try
            {
                DIPSbotHost dipsBot = Start();

                Console.ReadLine();

                Stop(dipsBot);
            }
            catch (Exception e)
			{
				Console.WriteLine("Could not start DIPSbot.");
                Console.WriteLine(e);
			}
		}

        static DIPSbotHost Start()
        {
            Console.WriteLine("Fetching configuration...");
            var configuration = ReadConfig();

            Console.WriteLine("Starting DIPSbot...");
            var dipsBot = new DIPSbotHost();
            dipsBot.Start(configuration);

            Console.WriteLine("DIPSbot started.");
            return dipsBot;
        }

        static void Stop(DIPSbotHost dipsBot)
		{
			var res = dipsBot.Stop().GetAwaiter().GetResult();
			Console.WriteLine(res);
			Console.WriteLine("DIPSbot stopped.");
		}

        static Configuration ReadConfig() {
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config.json"));
        }
	}
}