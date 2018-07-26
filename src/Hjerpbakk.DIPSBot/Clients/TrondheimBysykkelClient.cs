using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using BikeshareClient;
using Hjerpbakk.DIPSBot.Model;
using Hjerpbakk.DIPSBot.Model.BikeShare;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Hjerpbakk.DIPSBot.Clients {
    public class TrondheimBysykkelClient {
        readonly IMemoryCache memoryCache;
        readonly MemoryCacheEntryOptions cacheEntryOptions;

        readonly string baseQueryString;

        readonly Client bikeshareClient;

        public TrondheimBysykkelClient(IMemoryCache memoryCache, string googleMapsApiKey) {
            this.memoryCache = memoryCache;
            cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1));

            baseQueryString = "https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins={0}&destinations={1}&region=no&mode=walking&key=" + googleMapsApiKey;
            bikeshareClient = new Client("http://gbfs.urbansharing.com/trondheim/");
        }

        public async Task<Station> FindNearesBikeSharingStation(string userAddress) {
            // TODO: get nearest station based on users address and use this to select station
            // TODO: cache location info of stations
            var stations = await bikeshareClient.GetStationsAsync();

            // TODO: cache results based on address too
            userAddress = HttpUtility.UrlEncode("Beddingen 10");

            // TODO: for lolz, men kanskje nyttig å vise 2. og 3. alternativ?
            var allStations = new List<(long duration, BikeshareClient.Models.Station station)>();


            BikeshareClient.Models.Station nearestStation = null;
            var smallestDuration = long.MaxValue;
            foreach (var station in stations) {
                // TODO: From DI
                using (var httpClient = new HttpClient()) {
                    var queryString = string.Format(baseQueryString, userAddress, $"{station.Latitude},{station.Longitude}");
                    var response = await httpClient.GetStringAsync(queryString);
                    var route = JsonConvert.DeserializeObject<Route>(response);
                    // TODO: support more routes in result
                    // TODO: more robust error handling
                    Console.WriteLine(response);

                    var duration = route.Rows[0].Elements[0].Duration?.Value;
                    // TODO: for lolz
                    if (duration.HasValue) {
                        allStations.Add((duration.Value, station));
                    }

                    if (duration.HasValue && duration < smallestDuration) {
                        smallestDuration = duration.Value;
                        nearestStation = station;
                    }
                }
            }

            // TODO: for lolz
            var orderedStations = allStations.OrderBy(d => d.duration);

            // TODO: Could not find a station
            if (nearestStation == null) {
                return null;
            }

            var stationStatus = (await bikeshareClient.GetStationsStatusAsync()).Single(s => s.Id == nearestStation.Id);

            return new Station(nearestStation.Name,
                               nearestStation.Address,
                               stationStatus.BikesAvailable,
                               stationStatus.DocksAvailable,
                               nearestStation.Latitude,
                               nearestStation.Longitude);
        }

        async Task<T> GetOrSet<T>(object key, Func<Task<T>> create) {
            if (!memoryCache.TryGetValue(key, out T view)) {
                view = await create();
                memoryCache.Set(key, view, cacheEntryOptions);
            }

            return view;
        }
    }
}
