using System.IO;
using Hjerpbakk.DIPSBot.Configuration;
using Newtonsoft.Json;

namespace Tests.TestData {
    public static class CreateConfig {
        public static T OfType<T>() where T : IConfiguration {
            var json = File.ReadAllText("../../../TestData/config.json");
            var config = (IConfiguration)JsonConvert.DeserializeObject<AppConfiguration>(json);
            return (T)config;
        }
    }
}
