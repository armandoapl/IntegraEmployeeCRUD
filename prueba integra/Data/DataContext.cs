using Microsoft.EntityFrameworkCore;
using prueba_integra.Models;

namespace prueba_integra.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {   
        }

        public DbSet<Employee> Employees { get; set; }

    }
}