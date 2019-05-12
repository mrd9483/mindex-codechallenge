using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;

namespace challenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
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
            return _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
        }

        public Employee GetById(string id)
        {
            return _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        /// <summary>
        /// Adds compensation to the database
        /// </summary>
        /// <param name="compensation"></param>
        /// <returns></returns>
        public Compensation Add(Compensation compensation)
        {
            _employeeContext.Compensation.Add(compensation);
            return compensation;
        }

        /// <summary>
        /// Gets a compensation by employee Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Compensation> GetCompensationsById(string employeeId)
        {
            return _employeeContext.Compensation
                .Include(e => e.Employee)
                .Where(e => e.Employee.EmployeeId == employeeId)
                .OrderByDescending(c => c.EffectiveDate);
        }
    }
}
