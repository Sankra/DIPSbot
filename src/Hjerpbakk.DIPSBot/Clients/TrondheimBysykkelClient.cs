using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using BikeshareClient;
using Hjerpbakk.DIPSBot.Model.BikeShare;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Hjerpbakk.DIPSBot.Clients {
    public class TrondheimBysykkelClient {
        const int StationsCacheKey = 1;

        readonly HttpClient httpClient;
        readonly IMemoryCache memoryCache;
        readonly MemoryCacheEntryOptions cacheEntryOptions;

        readonly string baseQueryString;

        readonly Client bikeshareClient;

        public TrondheimBysykkelClient(HttpClient httpClient, IMemoryCache memoryCache, string googleMapsApiKey) {
            this.httpClient = httpClient;
            this.memoryCache = memoryCache;
            cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1));

            baseQueryString = "https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins={0}&destinations={1}&region=no&mode=walking&key=" + googleMapsApiKey;
            bikeshareClient = new Client("http://gbfs.urbansharing.com/trondheim/");
        }

        public async Task<BikeStation> FindNearesBikeSharingStation(string userAddress) {
            if (string.IsNullOrEmpty(userAddress)) {
                throw new ArgumentNullException(nameof(userAddress));
            }

            var encodedAddress = HttpUtility.UrlEncode(userAddress);
            return await GetOrSet(encodedAddress, FindNearestStation);

            async Task<BikeStation> FindNearestStation() {
                var allStationsInArea = await GetInformationOnAllStations();
                var queryString = string.Format(baseQueryString, encodedAddress, allStationsInArea.Coordinates);
                var response = await httpClient.GetStringAsync(queryString);
                var routes = JsonConvert.DeserializeObject<Route>(response);

                if (routes.Rows.Length == 0) {
                    return null;
                }

                var nearestStations = new SortedList<long, BikeshareClient.Models.Station>();
                var route = routes.Rows[0];
                for (int i = 0; i < route.Elements.Length; i++) {
                    var element = route.Elements[i];
                    if (element.Status != "OK") {
                        continue;
                    }

                    nearestStations.Add(element.Duration.Value, allStationsInArea.Stations[i]);
                }

                var nearestStation = nearestStations.First().Value;
                var stationStatus = allStationsInArea.StationsStatus.Single(s => s.Id == nearestStation.Id);
                return new BikeStation(nearestStation.Name,
                                   nearestStation.Address,
                                   stationStatus.BikesAvailable,
                                   stationStatus.DocksAvailable,
                                   nearestStation.Latitude,
                                   nearestStation.Longitude,
                                   nearestStations.First().Key);

                async Task<AllStationsInArea> GetInformationOnAllStations() {
                    var stationTask = GetOrSet(StationsCacheKey, async () => {
                        var stations = (await bikeshareClient.GetStationsAsync()).ToArray();
                        var coordinates = string.Join("|", stations.Select(station => $"{station.Latitude},{station.Longitude}").ToArray());
                        return (stations, coordinates);
                    });
                    var stationStatusTask = bikeshareClient.GetStationsStatusAsync();
                    await Task.WhenAll(stationTask, stationStatusTask);

                    if (stationTask.IsFaulted) {
                        throw stationTask.Exception;
                    }

                    if (stationStatusTask.IsFaulted) {
                        throw stationStatusTask.Exception;
                    }

                    var stationsAndCoordinates = stationTask.Result;
                    var stationsStatus = stationStatusTask.Result;
                    return new AllStationsInArea(stationsAndCoordinates.stations,
                                                 stationsAndCoordinates.coordinates,
                                                 stationsStatus);
                }
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
}
