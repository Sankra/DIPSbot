using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using BikeshareClient;
using Hjerpbakk.DIPSBot.Extensions;
using Hjerpbakk.DIPSBot.Model.BikeShare;
using Microsoft.Extensions.Caching.Memory;

namespace Hjerpbakk.DIPSBot.Clients {
    public class BikeShareClient {
        readonly IBikeshareClient bikeshareClient;

        readonly HttpClient httpClient;
        readonly IMemoryCache memoryCache;

        public BikeShareClient(IBikeshareClient bikeshareClient, HttpClient httpClient, IMemoryCache memoryCache) {
            this.httpClient = httpClient;
            this.bikeshareClient = bikeshareClient;
            this.memoryCache = memoryCache;
        }

        public async Task<AllStationsInArea> GetAllBikeSharingStations() {
            const int StationsCacheKey = 1337;
            var stationsTask = memoryCache.GetOrSet(StationsCacheKey, async () => {
                return await bikeshareClient.GetStationsAsync();
            });

            var stationsStatusTask = bikeshareClient.GetStationsStatusAsync();
            await Task.WhenAll(stationsTask, stationsStatusTask);

            VerifyTaskCompletedSuccessfully(stationsTask);
            VerifyTaskCompletedSuccessfully(stationsStatusTask);
            return new AllStationsInArea(stationsTask.Result, stationsStatusTask.Result);

            void VerifyTaskCompletedSuccessfully(Task task) {
                if (task.IsFaulted) {
                    throw task.Exception;
                }
            }
        }
    }
}
