using challenge.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Data
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {

        }

        /// <summary>
        /// Tells the context what the compensation model should entail, in terms of FK, etc.
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Compensation>()
                 .HasOne(c => c.Employee)
                 .WithMany()
                 .HasForeignKey(c => c.EmployeeId);

            base.OnModelCreating(builder);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Compensation> Compensation { get; set; }
    }
}
