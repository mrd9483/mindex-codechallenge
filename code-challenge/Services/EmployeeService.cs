using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;
using challenge.Types;
using challenge.Exceptions;

namespace challenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if (employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee GetDetailedById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetDetailedById(id);
            }

            return null;
        }

        public ReportingStructure GetNumberOfReports(string id)
        {
            var parent = GetDetailedById(id);

            if (parent != null)
            {
                return new ReportingStructure()
                {
                    Employee = parent,
                    NumberOfReports = GetNumberOfReports(parent, new List<Employee>())
                };
            }

            return null;
        }

        private int GetNumberOfReports(Employee employee, List<Employee> alreadyReferenced)
        {
            var retVal = 0;

            if(alreadyReferenced.Contains(employee))
            {
                throw new CircularReferenceException(employee);
            }

            alreadyReferenced.Add(employee);

            foreach(var e in employee.DirectReports)
            {
                retVal++;
                retVal += GetNumberOfReports(GetDetailedById(e.EmployeeId), alreadyReferenced);
            }

            return retVal;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if (originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }
    }
}
