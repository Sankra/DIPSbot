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
        readonly string serviceURL;

        readonly HttpClient httpClient;
        readonly Configuration configuration;

        public KitchenResponsibleClient(HttpClient httpClient, Configuration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            serviceURL = configuration.KitchenServiceURL + "/api/";
        }

        public async Task<(bool ok, string error)> AddEmployee(SlackUser employee)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(employee.Id);
				var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
				var response = await httpClient.PostAsync(serviceURL + "employee", content);
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
			var weeksAndEmployees = await httpClient.GetStringAsync(serviceURL + "kitchen");
            return JsonConvert.DeserializeObject<EmployeeWeek[]>(weeksAndEmployees);
        }

        public async Task<EmployeeWeek> GetNextWeekForEmployee(SlackUser employee)
        {
            var employeeAndWeek = await httpClient.GetStringAsync(serviceURL + "employee/" + employee.Id);
			return JsonConvert.DeserializeObject<EmployeeWeek>(employeeAndWeek);
        }

        public async Task<EmployeeWeek> GetResponsibleForWeek(ushort week)
        {
            var employeeAndWeek = await httpClient.GetStringAsync(serviceURL + "week/" + week);
			return JsonConvert.DeserializeObject<EmployeeWeek>(employeeAndWeek);
        }

		public async Task<EmployeeWeek> GetResponsibleForCurrentWeek()
		{
			var employeeAndWeek = await httpClient.GetStringAsync(serviceURL + "week");
            return JsonConvert.DeserializeObject<EmployeeWeek>(employeeAndWeek);
		}

        public async Task RemoveEmployee(SlackUser employee) =>
            await httpClient.DeleteAsync(serviceURL + "employee/" + employee.Id);
    }
}
