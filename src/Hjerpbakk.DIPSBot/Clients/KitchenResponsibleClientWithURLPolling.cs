using System;
using System.Net.Http;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Model;
using Newtonsoft.Json;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Clients
{
    public class KitchenResponsibleClientWithURLPolling : IKitchenResponsibleClient
    {
        readonly ServiceDiscoveryClient serviceDiscoveryClient;
        readonly IKitchenResponsibleClient kitchenResponsibleClient;

        public KitchenResponsibleClientWithURLPolling(HttpClient httpClient, IReadOnlyConfiguration configuration, ServiceDiscoveryClient serviceDiscoveryClient)
        {
			// TODO: Remove this class and replace the polling with events when available
			// https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-event-overview
			this.serviceDiscoveryClient = serviceDiscoveryClient;
            kitchenResponsibleClient = new KitchenResponsibleClient(httpClient, configuration);
        }

		public async Task AddEmployee(SlackUser employee)
		{
            try
            {
                await kitchenResponsibleClient.AddEmployee(employee);
            }
            catch (Exception)
            {
                await serviceDiscoveryClient.SetKitchenServiceURL();
                await kitchenResponsibleClient.AddEmployee(employee);
            }
		}

		public async Task<EmployeeWeek[]> GetAllWeeks()
		{
            try
            {
                return await kitchenResponsibleClient.GetAllWeeks();
            }
            catch (Exception)
            {
                await serviceDiscoveryClient.SetKitchenServiceURL();
                return await kitchenResponsibleClient.GetAllWeeks();
            }
		}

		public async Task<EmployeeWeek> GetNextWeekForEmployee(SlackUser employee)
		{
            try
            {
                return await kitchenResponsibleClient.GetNextWeekForEmployee(employee);
            }
            catch (Exception)
            {
                await serviceDiscoveryClient.SetKitchenServiceURL();
                return await kitchenResponsibleClient.GetNextWeekForEmployee(employee);
            }
		}

		public async Task<EmployeeWeek> GetResponsibleForWeek(ushort week)
		{
            try
            {
                return await kitchenResponsibleClient.GetResponsibleForWeek(week);
            }
            catch (Exception)
            {
                await serviceDiscoveryClient.SetKitchenServiceURL();
                return await kitchenResponsibleClient.GetResponsibleForWeek(week);
            }
		}

		public async Task<EmployeeWeek> GetResponsibleForCurrentWeek()
		{
            try
            {
                return await kitchenResponsibleClient.GetResponsibleForCurrentWeek();
            }
            catch (Exception)
            {
                await serviceDiscoveryClient.SetKitchenServiceURL();
                return await kitchenResponsibleClient.GetResponsibleForCurrentWeek();
            }
		}

		public async Task RemoveEmployee(SlackUser employee)
		{
            try
            {
                await kitchenResponsibleClient.RemoveEmployee(employee);
            }
            catch (Exception)
            {
                await serviceDiscoveryClient.SetKitchenServiceURL();
                await kitchenResponsibleClient.RemoveEmployee(employee);
            }
        }
    }
}
