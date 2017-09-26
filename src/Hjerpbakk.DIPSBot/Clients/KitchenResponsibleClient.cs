using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Model;
using Newtonsoft.Json;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Clients
{
    class KitchenResponsibleClient : IKitchenResponsibleClient
    {
        const string ServiceURL = "http://localhost:5000/api/";

        readonly HttpClient httpClient;

        public KitchenResponsibleClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<(bool ok, string error)> AddEmployee(SlackUser employee)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(employee.Id);
				var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
				var response = await httpClient.PostAsync(ServiceURL + "employee", content);
				if (response.StatusCode == HttpStatusCode.OK)
				{
					return (true, "");
				}

				return (false, "An error occurred: \n" + response);
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
        }

		public async Task<EmployeeWeek[]> GetAllWeeks()
        {
			var weeksAndEmployees = await httpClient.GetStringAsync(ServiceURL + "kitchen");
            return JsonConvert.DeserializeObject<EmployeeWeek[]>(weeksAndEmployees);
        }

        public async Task<EmployeeWeek> GetNextWeekForEmployee(SlackUser employee)
        {
            var employeeAndWeek = await httpClient.GetStringAsync(ServiceURL + "employee/" + employee.Id);
			return JsonConvert.DeserializeObject<EmployeeWeek>(employeeAndWeek);
        }

        public async Task<EmployeeWeek> GetResponsibleForWeek(ushort week)
        {
            var employeeAndWeek = await httpClient.GetStringAsync(ServiceURL + "week/" + week);
			return JsonConvert.DeserializeObject<EmployeeWeek>(employeeAndWeek);
        }

		public async Task<EmployeeWeek> GetResponsibleForCurrentWeek()
		{
			var employeeAndWeek = await httpClient.GetStringAsync(ServiceURL + "week");
            return JsonConvert.DeserializeObject<EmployeeWeek>(employeeAndWeek);
		}

        public Task RemoveEmployee(SlackUser employee)
        {
            throw new NotImplementedException();
        }
    }
}
