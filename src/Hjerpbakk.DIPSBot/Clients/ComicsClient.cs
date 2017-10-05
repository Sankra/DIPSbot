using System.Net.Http;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Configuration;

namespace Hjerpbakk.DIPSBot.Clients
{
    public class ComicsClient
    {
		readonly HttpClient httpClient;
		readonly IReadOnlyAppConfiguration configuration;

		public ComicsClient(HttpClient httpClient, IReadOnlyAppConfiguration configuration)
		{
			this.httpClient = httpClient;
			this.configuration = configuration;
		}

        public async Task<string> GetRandomComicAsync() =>
            await httpClient.GetStringAsync(configuration.ComicsServiceURL + "comics/random");
    }
}
