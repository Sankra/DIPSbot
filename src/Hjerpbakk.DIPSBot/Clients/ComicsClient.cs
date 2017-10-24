using System;
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

        public async Task<string> GetRandomComicAsync() {
            string comicURL;
            do
            {
                // TODO: Add breaker and default comic if this fails too often
                comicURL = await httpClient.GetStringAsync(configuration.ComicsServiceURL + "comics/random");
            } while (await ComicIsOffline(comicURL));

            return comicURL;
        }

        async Task<bool> ComicIsOffline(string comicURL) {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(comicURL);
                return !response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}
