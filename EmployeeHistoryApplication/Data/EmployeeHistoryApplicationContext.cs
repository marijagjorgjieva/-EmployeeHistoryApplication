using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmployeeHistoryApplication.Models;

namespace EmployeeHistoryApplication.Data
{
    public class EmployeeHistoryApplicationContext : DbContext
    {
        public EmployeeHistoryApplicationContext (DbContextOptions<EmployeeHistoryApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeHistoryApplication.Models.Employee> Employee { get; set; } = default!;
    }
}
