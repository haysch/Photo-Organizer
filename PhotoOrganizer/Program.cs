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
using System.Linq;
using System.Threading.Tasks;

namespace PhotoOrganizer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // setup configuration
            var configuration = BuildConfiguration(args);
            var interactive = args.Contains("--interactive");
            var database = !args.Contains("--no-database"); // if --no-database then false, else true

            var serviceProvider = ConfigureServices(configuration, interactive, database);

            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var organizerService = services.GetRequiredService<IOrganizerService>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            var inputPath = configuration.GetValue<string>("input");
            if (string.IsNullOrEmpty(inputPath))
            {
                logger.LogError($"No input path is specified. Use -i|--input /path/to/input/dir. Exiting.");
                return;
            }
            else if (!Directory.Exists(inputPath))
            {
                throw new DirectoryNotFoundException($"Input directory {inputPath} not found. Exiting.");
            }
            else
            {
                await organizerService.RunOrganizerAsync(inputPath, database);
            }
        }

        static IConfiguration BuildConfiguration(string[] args)
        {
            var switchMappings = new Dictionary<string, string>
            {
                { "-i", "input" },
                { "-o", "output" },
                { "-d", "database" },
                { "-r", "rename-type" },
                { "-h", "hash-algorithm" }
            };

            return new ConfigurationBuilder()
                // .AddEnvironmentVariables(PREFIX) // Needs more refinement
                .AddCommandLine(args, switchMappings)
                .Build();
        }

        static IServiceProvider ConfigureServices(IConfiguration configuration, bool interactive, bool database)
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
            if (database)
            {
                services.AddDbContext<PhotoContext>(options =>
                {
                    string connectionString = DatabaseUtil.ConstructDbConnectionString(configuration, databaseFlag, consoleWrapper, interactive);
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
            }

            // Setup DI for services
            services.AddSingleton(configuration)
                .AddSingleton<IRenameService, RenameService>()
                .AddSingleton<ISortService, SortService>()
                .AddSingleton<IOrganizerService, OrganizerService>();

            return services.BuildServiceProvider();
        }
    }
}
