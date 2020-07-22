using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhotoOrganizer.Enums;
using PhotoOrganizerLib.Data;
using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PhotoOrganizer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = BuildConfiguration(args);
            var serviceProvider = ConfigureServices(configuration);

            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var organizerService = services.GetRequiredService<IOrganizerService>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            var inputPath = configuration.GetValue<string>("input");
            if (inputPath is null)
            {
                logger.LogError($"No input path is specified. Use [--input|-i] /path/to/input/dir parameter. Exiting.");
                return;
            }
            else
            {
                await organizerService.RunOrganizerAsync(inputPath);
            }
        }

        static IConfiguration BuildConfiguration(string[] args)
        {
            var switchMappings = new Dictionary<string, string>
            {
                { "-i", "input" },
                { "-o", "output" },
                { "-db", "database" },
                { "-rt", "rename-type" },
                { "-ha", "hash-algorithm" }
            };

            return new ConfigurationBuilder()
                // .AddEnvironmentVariables(PREFIX) // Needs more refinement
                .AddCommandLine(args, switchMappings)
                .Build();
        }

        static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.AddLogging(logger => 
            {
                logger.ClearProviders();
                logger.AddConsole();
            });

            // Configure database
            var databaseFlag = configuration.GetValue<DatabaseFlag>("database");
            services.AddDbContext<PhotoContext>(options =>
            {
                switch (databaseFlag)
                {
                    case DatabaseFlag.MySQL:
                        // options.UseMySQL("");
                        break;
                    case DatabaseFlag.PostgreSQL:
                        // options.UseNpgsql("");
                        break;
                    case DatabaseFlag.SQLite:
                    default:
                        var outputPath = configuration.GetValue<string>("output") ?? Directory.GetCurrentDirectory();
                        var dbPath = Path.Combine(outputPath, "PhotoOrganizer.db");
                        options.UseSqlite($"Data Source={dbPath}");
                        break;
                }
            });

            // Setup DI for services
            services.AddSingleton<IConfiguration>(configuration)
                .AddSingleton<IRenameService, RenameService>()
                .AddSingleton<ISortService, SortService>()
                .AddSingleton<IOrganizerService, OrganizerService>();

            return services.BuildServiceProvider();
        }
    }
}
