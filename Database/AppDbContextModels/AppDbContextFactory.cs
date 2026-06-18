using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.AppDbContextModels
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var connectionString =
                Environment.GetEnvironmentVariable("TRAIN_CONNECTION")
                ?? "server=localhost;port=3306;database=training;user=root;password=root;";

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 32));

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(connectionString, serverVersion)
                .Options;

            return new AppDbContext(options);
        }
    }
}
