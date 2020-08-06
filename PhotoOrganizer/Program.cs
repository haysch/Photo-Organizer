using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhotoOrganizer.Enums;
using PhotoOrganizer.Utils;
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
            if (string.IsNullOrEmpty(inputPath))
            {
                logger.LogError($"No input path is specified. Use --input|-i /path/to/input/dir. Exiting.");
                return;
            }
            else if (!Directory.Exists(inputPath))
            {
                throw new DirectoryNotFoundException($"Input directory {inputPath} not found. Exiting.");
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

            // Create ConsoleWrapper for separation of concern
            var consoleWrapper = new ConsoleWrapper();
            // Configure database
            var databaseFlag = configuration.GetValue<DatabaseFlag>("database");
            services.AddDbContext<PhotoContext>(options =>
            {
                string connectionString = DatabaseUtil.ConstructDbConnectionString(configuration, databaseFlag, consoleWrapper);
                switch (databaseFlag)
                {
                    case DatabaseFlag.SQLServer:
                        options.UseSqlServer(connectionString);
                        break;
                    case DatabaseFlag.MySQL:
                        options.UseMySQL(connectionString);
                        break;
                    case DatabaseFlag.PostgreSQL:
                        options.UseNpgsql(connectionString);
                        break;
                    case DatabaseFlag.SQLite:
                    default:
                        options.UseSqlite(connectionString);
                        break;
                }
            });

            // Setup DI for services
            services.AddSingleton(configuration)
                .AddSingleton<IRenameService, RenameService>()
                .AddSingleton<ISortService, SortService>()
                .AddSingleton<IOrganizerService, OrganizerService>();

            return services.BuildServiceProvider();
        }
    }
}
