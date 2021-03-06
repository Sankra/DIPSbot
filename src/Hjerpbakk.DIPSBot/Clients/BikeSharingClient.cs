﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BikeshareClient;
using Hjerpbakk.DIPSBot.Extensions;
using Hjerpbakk.DIPSBot.Model.BikeSharing;
using Microsoft.Extensions.Caching.Memory;

namespace Hjerpbakk.DIPSBot.Clients {
    public class BikeSharingClient {
        readonly IBikeshareClient bikeshareClient;
        readonly IMemoryCache memoryCache;

        public BikeSharingClient(IBikeshareClient bikeshareClient, IMemoryCache memoryCache) {
            this.bikeshareClient = bikeshareClient;
            this.memoryCache = memoryCache;
        }

        public async Task<AllStationsInArea> GetBikeSharingStationsMatchingIntention(Intention intention) {
            const int StationsCacheKey = 1337;
            var stationsTask = memoryCache.GetOrSet(StationsCacheKey, async () => {
                return await bikeshareClient.GetStationsAsync();
            });

            // TODO: filter out stations with status installed, renting, returning
            var stationsStatusTask = bikeshareClient.GetStationsStatusAsync();
            await Task.WhenAll(stationsTask, stationsStatusTask);

            VerifyTaskCompletedSuccessfully(stationsTask);
            VerifyTaskCompletedSuccessfully(stationsStatusTask);

            IEnumerable<string> idsToFilter;
            switch (intention) {
                case Intention.DropOff:
                    idsToFilter = stationsStatusTask.Result.Where(s => s.DocksAvailable == 0).Select(s => s.Id);
                    break;
                case Intention.PickUp:
                    idsToFilter = stationsStatusTask.Result.Where(s => s.BikesAvailable == 0).Select(s => s.Id);
                    break;
                default:
                    idsToFilter = new string[0];
                    break;
            }

            return new AllStationsInArea(stationsTask.Result.Where(s => !idsToFilter.Contains(s.Id)),
                                         stationsStatusTask.Result.Where(s => !idsToFilter.Contains(s.Id)));

            void VerifyTaskCompletedSuccessfully(Task task) {
                if (task.IsFaulted) {
                    throw task.Exception;
                }
            }
        }
    }
}
