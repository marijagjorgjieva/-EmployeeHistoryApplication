using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmployeeHistoryApplication.Models;
using System.Reflection.Metadata;

namespace EmployeeHistoryApplication.Data
{
    public class EmployeeHistoryApplicationContext : DbContext
    {
        public EmployeeHistoryApplicationContext (DbContextOptions<EmployeeHistoryApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.jobs)
                .WithOne(e => e.Employee)
                .HasForeignKey(e => e.EmployeeId)
                .IsRequired();
        }
        public DbSet<EmployeeHistoryApplication.Models.Employee> Employee { get; set; } = default!;
        public DbSet<EmployeeHistoryApplication.Models.JobHistory> JobHistory { get; set; } = default!;
    }
}
