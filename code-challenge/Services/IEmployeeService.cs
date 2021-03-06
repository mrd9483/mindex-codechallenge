﻿using challenge.Domain;
using challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Services
{
    public interface IEmployeeService
    {
        Employee GetById(String id);
        Employee GetDetailedById(String id);
        Employee Create(Employee employee);
        Employee Replace(Employee originalEmployee, Employee newEmployee);
        ReportingStructure GetNumberOfReports(string id);
        Compensation Create(Compensation compensation);
        IEnumerable<Compensation> GetCompensationsById(string employeeId);
    }
}
