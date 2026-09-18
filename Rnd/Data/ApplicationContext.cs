using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Rnd.Models;
using Serilog;

namespace Rnd.Data
{
    public class ApplicationContext : DbContext
    {
        //public DbSet<Person> Persons { get; set; } = null!;
        public ApplicationContext() { }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                Log.Information("Configuring DbContext with SQLite connection string from appsettings.json");
                var configuration = GetConfiguration();
                Log.Information($"Configuration Complete");

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlite(connectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Log.Information($"Applying Model Configuration");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
            Log.Information($"Configuration Applied");

            var configuration = GetConfiguration();
            var environment = configuration.GetValue<string>("Environment");
            if (environment == "Development")
            {
                SeedData(modelBuilder);
            }
        }
        private void SeedData(ModelBuilder modelBuilder)
        {
            Log.Information($"Seeding Data");
            modelBuilder.Entity<Person>().HasData(
                new Person
                {
                    Id = 1,
                    FirstName = "Alex",
                    LastName = "Terrible",
                    Email = "Alex@example.com",
                    IsDeleted = false,
                },
                new Person
                {
                    Id = 2,
                    FirstName = "Ariana",
                    LastName = "Grande",
                    Email = "Ariana@example.com",
                    IsDeleted = false,
                }
            );
            Log.Information($"Data Seeded");
        }
        private IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
