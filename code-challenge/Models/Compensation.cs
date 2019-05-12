using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace challenge.Models
{
    public class Compensation
    {
        [Key]
        public string CompensationId { get; set; }

        public string EmployeeId { get; set; }
        public decimal Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
