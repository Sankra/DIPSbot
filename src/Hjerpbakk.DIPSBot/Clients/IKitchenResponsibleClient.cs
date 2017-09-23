using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Model;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Clients
{
    interface IKitchenResponsibleClient
    {
        Task<EmployeeWeek[]> GetAllWeeks();
        Task<EmployeeWeek> GetResponsibleForWeek(ushort week);
        Task<EmployeeWeek> GetNextWeekForEmployee(SlackUser employee);
        Task<(bool ok, string error)> AddEmployee(SlackUser employee);
        Task RemoveEmployee(SlackUser employee);
    }
}
