using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;
using challenge.Exceptions;
using challenge.Domain;

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

        /// <summary>
        /// Gets an employee with direct reports. EF does a shallow retrieval unless you explicitly include.
        /// 
        /// Note that this will not recursively go through all users.
        /// </summary>
        /// <param name="id">the employee id</param>
        /// <returns>the employee, with direct reports</returns>
        public Employee GetDetailedById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetDetailedById(id);
            }

            return null;
        }

        /// <summary>
        /// Gets the number of direct and indirect reports, given an employee id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a compensation data point
        /// </summary>
        /// <param name="compensation">the compensation data</param>
        /// <exception cref="ArgumentException">If Employee not found</exception>
        /// <returns></returns>
        public Compensation Create(Compensation compensation)
        {
            var employee = GetById(compensation.EmployeeId);
            if(employee == null)
            {
                throw new ArgumentException("Referenced Employee id not found in system.", "EmployeeId");
            }

            if (compensation != null)
            {
                _employeeRepository.Add(compensation);
                _employeeRepository.SaveAsync().Wait();
            }

            return compensation;
        }

        /// <summary>
        /// Retrieves the compensation by employee id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Compensation> GetCompensationsById(string employeeId)
        {
            if (!string.IsNullOrEmpty(employeeId))
            {
                return _employeeRepository.GetCompensationsById(employeeId);
            }

            return null;
        }

        /// <summary>
        /// Gets the number of reports given an employee. Checks for circular dependencies as well.
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="alreadyReferenced"></param>
        /// <returns></returns>
        private int GetNumberOfReports(Employee employee, List<Employee> alreadyReferenced)
        {
            var retVal = 0;

            if (alreadyReferenced.Contains(employee))
            {
                throw new CircularReferenceException(employee);
            }

            alreadyReferenced.Add(employee);

            foreach (var e in employee.DirectReports)
            {
                retVal++;
                //EF doesn't recursively retrieve all direct report's direct reports, so we have to call a detailed user call each time.
                retVal += GetNumberOfReports(GetDetailedById(e.EmployeeId), alreadyReferenced);
            }

            return retVal;
        }
    }
}
