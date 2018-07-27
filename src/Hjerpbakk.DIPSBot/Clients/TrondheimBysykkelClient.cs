using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using BikeshareClient;
using BikeshareClient.Models;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Model.BikeShare;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Hjerpbakk.DIPSBot.Clients {
    public class TrondheimBysykkelClient {
        readonly HttpClient httpClient;
        readonly string baseDistanceQueryString;
        readonly string baseImageQueryString;

        readonly Client bikeshareClient;

        readonly IMemoryCache memoryCache;
        readonly MemoryCacheEntryOptions cacheEntryOptions;

        public TrondheimBysykkelClient(HttpClient httpClient, IGoogleMapsConfiguration googleMapsConfiguration, IMemoryCache memoryCache) {
            this.httpClient = httpClient;
            baseDistanceQueryString = "https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins={0}&destinations={1}&region=no&mode=walking&key=" + googleMapsConfiguration.GoogleMapsApiKey;
            // TODO: Add real locations
            // TODO: Add route
            // TODO: Decide for or against custom icon
            // TODO: Decide size and scale factor...
            baseImageQueryString = "https://maps.googleapis.com/maps/api/staticmap?size=600x600&scale=2&maptype=roadmap&region=no&markers=color:green%7Clabel:A%7CBeddingen+10&markers=icon:https://hjerpbakk.com/assets/img/parking.png%7C63.435399485821023,10.409983098506927&key=" + googleMapsConfiguration.GoogleMapsApiKey;
            this.memoryCache = memoryCache;
            cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1));
            bikeshareClient = new Client("http://gbfs.urbansharing.com/trondheim/");
        }

        public async Task<BikeStation> FindNearesBikeSharingStation(string userAddress) {
            if (string.IsNullOrEmpty(userAddress)) {
                throw new ArgumentNullException(nameof(userAddress));
            }

            return await FindNearestStation();

            async Task<BikeStation> FindNearestStation() {
                var allStationsInArea = await GetInformationOnAllStations();
                var encodedAddress = HttpUtility.UrlEncode(userAddress);
                var routes = await GetOrSet(encodedAddress, FindRoutesToAllStations);

                var sortedStations = SortStationsByDistanceFromUser();
                var nearestStation = sortedStations.First().Value;
                var stationStatus = allStationsInArea.StationsStatus.Single(s => s.Id == nearestStation.Id);
                return new BikeStation(nearestStation.Name,
                                   nearestStation.Address,
                                   stationStatus.BikesAvailable,
                                   stationStatus.DocksAvailable,
                                   nearestStation.Latitude,
                                   nearestStation.Longitude,
                                   sortedStations.First().Key);

                async Task<AllStationsInArea> GetInformationOnAllStations() {
                    const int StationsCacheKey = 1337;
                    var stationTask = GetOrSet(StationsCacheKey, async () => {
                        var stations = (await bikeshareClient.GetStationsAsync()).ToArray();
                        var coordinates = HttpUtility.UrlEncode(string.Join("|", stations.Select(station => $"{station.Latitude},{station.Longitude}").ToArray()));
                        return (stations, coordinates);
                    });
                    var stationStatusTask = bikeshareClient.GetStationsStatusAsync();
                    await Task.WhenAll(stationTask, stationStatusTask);

                    VerifyTaskCompletedSuccessfully(stationTask);
                    VerifyTaskCompletedSuccessfully(stationStatusTask);

                    return new AllStationsInArea(stationTask.Result.stations,
                                                 stationTask.Result.coordinates,
                                                 stationStatusTask.Result);

                    void VerifyTaskCompletedSuccessfully(Task task) {
                        if (task.IsFaulted) {
                            throw task.Exception;
                        }
                    }
                }

                async Task<Element[]> FindRoutesToAllStations() {
                    var queryString = string.Format(baseDistanceQueryString, encodedAddress, allStationsInArea.Coordinates);
                    var response = await httpClient.GetStringAsync(queryString);
                    var route = JsonConvert.DeserializeObject<Route>(response);

                    if (route.Rows.Length == 0) {
                        throw new InvalidOperationException($"Could not find any routes from {userAddress} to any bike sharing stations.");
                    }

                    return route.Rows[0].Elements;
                }

                SortedList<long, Station> SortStationsByDistanceFromUser() {
                    var nearestStations = new SortedList<long, Station>();
                    for (int i = 0; i < routes.Length; i++) {
                        var element = routes[i];
                        if (element.Status != "OK") {
                            continue;
                        }

                        nearestStations.Add(element.Duration.Value, allStationsInArea.Stations[i]);
                    }

                    return nearestStations;
                }
            }

            async Task<T> GetOrSet<T>(object key, Func<Task<T>> create) {
                if (!memoryCache.TryGetValue(key, out T result)) {
                    result = await create();
                    memoryCache.Set(key, result, cacheEntryOptions);
                }

                return result;
            }
        }
    }
}
