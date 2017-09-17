using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Model;
using Newtonsoft.Json;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Clients
{
    class KitchenResponsibleClient : IKitchenResponsibleClient
    {
        readonly HttpClient httpClient;

        public KitchenResponsibleClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task AddEmployee(SlackUser employee)
        {
            throw new NotImplementedException();
        }

		public async Task<EmployeeWeek[]> GetAllWeeks()
        {
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));

			var weeksAndEmployees = await httpClient.GetStringAsync("http://localhost:5000/api/values");

            return JsonConvert.DeserializeObject<EmployeeWeek[]>(weeksAndEmployees);
        }

        public Task<EmployeeWeek> GetNextWeekForEmployee(SlackUser employee)
        {
            throw new NotImplementedException();
        }

        public Task<EmployeeWeek> GetResponsibleForWeek(ushort week)
        {
            throw new NotImplementedException();
        }

        public Task RemoveEmployee(SlackUser employee)
        {
            throw new NotImplementedException();
        }
    }
}
