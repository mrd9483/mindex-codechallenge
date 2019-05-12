using challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Exceptions
{
    public class CircularReferenceException : Exception
    {
        public CircularReferenceException(Employee employee) : base($"Circular reference detected, for {employee.EmployeeId}") { }
    }
}
