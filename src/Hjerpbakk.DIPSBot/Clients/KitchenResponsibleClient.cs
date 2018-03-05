using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Model;
using Newtonsoft.Json;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Clients
{
    class KitchenResponsibleClient : IKitchenResponsibleClient
    {
        readonly HttpClient httpClient;
        readonly IReadOnlyAppConfiguration configuration;

        public KitchenResponsibleClient(HttpClient httpClient, IReadOnlyAppConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
        }

        public async Task AddEmployee(Employee employee)
        {
			var jsonContent = JsonConvert.SerializeObject(employee);
			var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
			var response = await httpClient.PostAsync(configuration.KitchenServiceURL + "employee", content);
			if (response.StatusCode == HttpStatusCode.OK)
			{
                return;
			}

            throw new Exception("An error occurred: \n" + response);
        }

        public async Task<EmployeeWeek[]> GetAllWeeks()
        {
            var weeksAndEmployees = await httpClient.GetStringAsync(configuration.KitchenServiceURL + "kitchen");
            return JsonConvert.DeserializeObject<EmployeeWeek[]>(weeksAndEmployees);
        }

        // TODO: KitchenServiceURL ble satt til noe annet. forbedre og si fra når blir endret
        public async Task<EmployeeWeek> GetNextWeekForEmployee(SlackUser employee)
        {
            var employeeAndWeek = await httpClient.GetStringAsync(configuration.KitchenServiceURL + "employee/" + employee.Id);
            return JsonConvert.DeserializeObject<EmployeeWeek>(employeeAndWeek);
        }

        public async Task<EmployeeWeek> GetResponsibleForWeek(ushort week)
        {
            var employeeAndWeek = await httpClient.GetStringAsync(configuration.KitchenServiceURL + "week/" + week);
            return JsonConvert.DeserializeObject<EmployeeWeek>(employeeAndWeek);
        }

        public async Task<EmployeeWeek> GetResponsibleForCurrentWeek()
        {
            var employeeAndWeek = await httpClient.GetStringAsync(configuration.KitchenServiceURL + "week");
            return JsonConvert.DeserializeObject<EmployeeWeek>(employeeAndWeek);
        }

        public async Task RemoveEmployee(SlackUser employee)
        {
            await httpClient.DeleteAsync(configuration.KitchenServiceURL + "employee/" + employee.Id);
        }
    }
}
