﻿using System;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Model;
using SlackConnector.Models;

namespace Hjerpbakk.DIPSBot.Clients
{
    interface IKitchenResponsibleClient
    {
        Task<EmployeeWeek[]> GetAllWeeks();
        Task<EmployeeWeek> GetResponsibleForCurrentWeek();
        Task<EmployeeWeek> GetResponsibleForWeek(ushort week);
        Task<EmployeeWeek> GetNextWeekForEmployee(SlackUser employee);
        Task AddEmployee(Employee employee);
        Task RemoveEmployee(SlackUser employee);
    }
}
