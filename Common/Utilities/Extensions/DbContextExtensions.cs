using Microsoft.Extensions.Configuration;
using Utilities.Models;

namespace Utilities.Extensions;

public class DbContextExtensions
{
    public static Options GetOptions(string connectionStringName)
    {
        var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false);

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (!string.IsNullOrEmpty(environment))
            builder.AddJsonFile($"appsettings.{environment}.json");

        var configuration = builder.Build();

        return new()
        {
            ConnectionString = configuration.GetConnectionString(connectionStringName),
            LogEnabled = configuration.GetSection("LogEnabled").Get<bool>()
        };
    }
}