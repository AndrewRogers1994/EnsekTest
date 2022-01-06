using ENSEK_Web_API.Models.Database;
using ENSEK_Web_API.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ENSEK_Web_API.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(ILogger<DatabaseContext> logger, DbContextOptions<DatabaseContext> options) : base(options)
        {
            //Check if the database is being created so that we can populate it with test data
            if (Database.EnsureCreated())
            {
                //Try to get the test accounts
                List<Account> testAccounts = CsvUtils.GetTestAccounts();

                //If there is test accounts to add to the database then go ahead and add them
                if (testAccounts.Count > 0)
                {
                    Accounts.AddRange(testAccounts);
                    SaveChanges();
                    logger?.LogInformation("Database Created and populated with Test Accounts.");
                }
                else
                {
                    logger?.LogWarning("Database could not be populated with test accounts. Is the file missing? Database created without test data.");
                }
            }
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Reading> Readings { get; set; }
    }
}