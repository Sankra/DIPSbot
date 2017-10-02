using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Model;
using Newtonsoft.Json;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Clients
{
    class KitchenResponsibleClient : IKitchenResponsibleClient
    {
        readonly HttpClient httpClient;
        readonly IReadOnlyConfiguration configuration;

        public KitchenResponsibleClient(HttpClient httpClient, IReadOnlyConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
        }

        string ServiceURL => configuration.KitchenServiceURL + "api/";

        public async Task AddEmployee(SlackUser employee)
        {
			var jsonContent = JsonConvert.SerializeObject(employee.Id);
			var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
			var response = await httpClient.PostAsync(ServiceURL + "employee", content);
			if (response.StatusCode == HttpStatusCode.OK)
			{
                return;
			}

            throw new Exception("An error occurred: \n" + response);
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

        public async Task RemoveEmployee(SlackUser employee)
        {
            await httpClient.DeleteAsync(ServiceURL + "employee/" + employee.Id);
        }
    }
}
