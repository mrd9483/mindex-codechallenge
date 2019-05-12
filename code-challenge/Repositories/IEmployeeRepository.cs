using challenge.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace challenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);
        Employee GetDetailedById(String id);
        Employee Add(Employee employee);
        Employee Remove(Employee employee);
        Task SaveAsync();
        Compensation Add(Compensation employee);
        IEnumerable<Compensation> GetCompensationsById(string id);
    }
}