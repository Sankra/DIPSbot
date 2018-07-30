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
            var stationTask = memoryCache.GetOrSet(StationsCacheKey, async () => {
                var stations = (await bikeshareClient.GetStationsAsync()).ToArray();
                var coordinates = HttpUtility.UrlEncode(string.Join("|", stations.Select(station => $"{station.Latitude},{station.Longitude}").ToArray()));
                return (stations, coordinates);
            });
            var stationStatusTask = bikeshareClient.GetStationsStatusAsync();
            await Task.WhenAll(stationTask, stationStatusTask);

            VerifyTaskCompletedSuccessfully(stationTask);
            VerifyTaskCompletedSuccessfully(stationStatusTask);

            return new AllStationsInArea(stationTask.Result.stations,
                                         stationStatusTask.Result.ToArray(),
                                         stationTask.Result.coordinates);

            void VerifyTaskCompletedSuccessfully(Task task) {
                if (task.IsFaulted) {
                    throw task.Exception;
                }
            }
        }
    }
}
