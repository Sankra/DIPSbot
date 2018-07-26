using System;
using System.Linq;
using System.Threading.Tasks;
using BikeshareClient;
using Hjerpbakk.DIPSBot.Model;
using Microsoft.Extensions.Caching.Memory;

namespace Hjerpbakk.DIPSBot.Clients {
    public class TrondheimBysykkelClient {
        readonly IMemoryCache memoryCache;
        readonly Client bikeshareClient;

        public TrondheimBysykkelClient(IMemoryCache memoryCache) {
            this.memoryCache = memoryCache;
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1));

            bikeshareClient = new Client("http://gbfs.urbansharing.com/trondheim/");
        }

        public async Task<BikeShareStation> FindNearesBikeSharingStation(string userAddress) {
            // TODO: get nearest station based on users address and use this to select station
            // TODO: cache location info of stations
            var station = (await bikeshareClient.GetStationsAsync()).Single(s => s.Address == userAddress);

            var stationStatus = (await bikeshareClient.GetStationsStatusAsync()).Single(s => s.Id == station.Id);

            return new BikeShareStation(station.Name, station.Address, stationStatus.BikesAvailable, stationStatus.DocksAvailable);
        }

        async Task<T> GetOrSet<T>(object key, Func<Task<T>> create) {
            if (!memoryCache.TryGetValue(key, out T view)) {
                view = await create();
                memoryCache.Set(key, view, options);
            }

            return view;
        }
    }
}
