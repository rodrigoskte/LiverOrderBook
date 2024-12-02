using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LiveOrderBook.Infrastructure.Context;

public class LiveOrderBookApiDbContextFactory: IDesignTimeDbContextFactory<LiveOrderBookApiDbContext>
{
    public LiveOrderBookApiDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../LiveOrderBook.Presentation.API"));
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<LiveOrderBookApiDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultSqlConnection_dev");

        optionsBuilder.UseSqlServer(connectionString);

        return new LiveOrderBookApiDbContext(optionsBuilder.Options);
    }
}