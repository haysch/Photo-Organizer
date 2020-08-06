using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Npgsql;
using PhotoOrganizer.Enums;
using PhotoOrganizer.Models;
using System;
using System.Data.Common;
using System.IO;
using System.Text;

namespace PhotoOrganizer.Utils
{
    /// <summary>
    /// Utility class for constructing database connection strings.
    /// </summary>
    public static class DatabaseUtil
    {
        private static readonly ILogger _logger = Logging.CreateLogger(typeof(DatabaseUtil).FullName);
        /// <summary>
        /// Entry function for constructing database connection string.
        /// Depending on the <see cref="DatabaseFlag"/> a PostgreSQL, MySQL or SQLite connection string is constructed.
        /// </summary>
        /// <param name="configuration">Configuration map containing a potential connection string.</param>
        /// <param name="databaseFlag">Flag used to decide what connection string to construct.</param>
        /// <param name="consoleWrapper">Wrapper for providing <see cref="Console"/> functionality.</param>
        /// <param name="interactive">Optional flag used for constructing a connection string interactively if none is provided.</param>
        /// <returns>Connection string for the chosen database.</returns>
        /// <exception cref="ArgumentException">If the configuration does not containg a "ConnectionString" value and --interactive is not provided.</exception>
        public static string ConstructDbConnectionString(IConfiguration configuration, DatabaseFlag databaseFlag, IConsoleWrapper consoleWrapper, bool interactive = false)
        {
            var connectionString = configuration.GetValue<string>("connectionstring");

            if (!string.IsNullOrEmpty(connectionString))
            {
                try
                {
                    new DbConnectionStringBuilder
                    {
                        ConnectionString = connectionString
                    };
                    return connectionString;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                return null;
            }
            else if (!interactive)
            {
                var db = Enum.GetName(typeof(DatabaseFlag), databaseFlag);
                throw new ArgumentException($"Unable to construct connection string for {db} due to --interactive is not used.");
            }

            // connection string is not entered and interactive mode is active
            return databaseFlag switch
            {
                DatabaseFlag.MySQL => ConstructMySqlConnectionString(consoleWrapper),
                DatabaseFlag.PostgreSQL => ConstructNpgsqlConnectionString(consoleWrapper),
                DatabaseFlag.SQLServer => ConstructSqlServerConnectionString(consoleWrapper),
                _ => ConstructSqliteConnectionString(configuration, consoleWrapper) // SQLite and default
            };
        }

        /// <summary>
        /// Constructs a MySql Connection String based on user input.
        /// </summary>
        /// <param name="consoleWrapper">Wrapper for Console functionality.</param>
        /// <returns>Connection string for MySql database.</returns>
        private static string ConstructMySqlConnectionString(IConsoleWrapper consoleWrapper)
        {
            var csInfo = GatherConnectionStringInfo(consoleWrapper);

            var mysqlCsBuilder = new MySqlConnectionStringBuilder
            {
                UserID = csInfo.UserId,
                Password = csInfo.Password,
                Server = csInfo.Server,
                Port = (uint)csInfo.Port,
                Database = csInfo.Database
            };

            return mysqlCsBuilder.ConnectionString;
        }

        /// <summary>
        /// Constructs a Npgsql Connection String based on user input.
        /// </summary>
        /// <param name="consoleWrapper">Wrapper for Console functionality.</param>
        /// <returns>Connection string for Npgsql database.</returns>
        private static string ConstructNpgsqlConnectionString(IConsoleWrapper consoleWrapper)
        {
            var csInfo = GatherConnectionStringInfo(consoleWrapper);

            var npgsqlCsBuilder = new NpgsqlConnectionStringBuilder
            {
                Username = csInfo.UserId,
                Password = csInfo.Password,
                Host = csInfo.Server,
                Port = csInfo.Port,
                Database = csInfo.Database
            };

            return npgsqlCsBuilder.ConnectionString;
        }

        /// <summary>
        /// Constructs a SQLServer Connection String based on user input.
        /// </summary>
        /// <param name="consoleWrapper">Wrapper for Console functionality.</param>
        /// <returns>Connection string for SQLServer database.</returns>
        private static string ConstructSqlServerConnectionString(IConsoleWrapper consoleWrapper)
        {
            var csInfo = GatherConnectionStringInfo(consoleWrapper);

            var sqlServerCsBuilder = new SqlConnectionStringBuilder
            {
                UserID = csInfo.UserId,
                Password = csInfo.Password,
                DataSource = $"{csInfo.Server},{csInfo.Port}",
                InitialCatalog = csInfo.Database
            };

            return sqlServerCsBuilder.ConnectionString;
        }

        private static ConnectionStringInfo GatherConnectionStringInfo(IConsoleWrapper consoleWrapper)
        {
            var connectionStringInfo = new ConnectionStringInfo();

            // Ask about login info
            consoleWrapper.Write("User ID: ");
            var userId = consoleWrapper.ReadLine();

            consoleWrapper.Write("Password: ");
            var password = BuildPasswordString(consoleWrapper);

            connectionStringInfo.UserId = userId;
            connectionStringInfo.Password = password;

            // Ask the standard questions
            consoleWrapper.Write("Server: ");
            var server = consoleWrapper.ReadLine();

            consoleWrapper.Write("Port: ");

            int port;
            while (!int.TryParse(consoleWrapper.ReadLine(), out port))
            {
                consoleWrapper.WriteLine("Invalid input port. Try again.");
            }

            consoleWrapper.Write("Database: ");
            var database = consoleWrapper.ReadLine();

            connectionStringInfo.Server = server;
            connectionStringInfo.Port = port;
            connectionStringInfo.Database = database;

            return connectionStringInfo;
        }

        /// <summary>
        /// Build a password without showing input.
        /// </summary>
        /// <param name="consoleWrapper">Wrapper for providing <see cref="Console"/> functionality.</param>
        /// <returns>Password string.</returns>
        private static string BuildPasswordString(IConsoleWrapper consoleWrapper)
        {
            var passwordBuilder = new StringBuilder();

            // loop until Enter is read
            ConsoleKeyInfo keyInfo;
            while ((keyInfo = consoleWrapper.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (keyInfo.Key == ConsoleKey.Backspace && passwordBuilder.Length > 0)
                {
                    // Remove last character
                    passwordBuilder.Length--;
                }
                else if (32 <= keyInfo.KeyChar && keyInfo.KeyChar <= 126)
                {
                    Console.Write(keyInfo.KeyChar);
                    passwordBuilder.Append(keyInfo.KeyChar);
                }
            }

            return passwordBuilder.ToString();
        }

        /// <summary>
        /// Method for constructing a SQLite data source string.
        /// </summary>
        /// <param name="configuration">Configuration map containing "output" and potentially "database-name".</param>
        /// <param name="consoleWrapper">Wrapper for providing <see cref="Console"/> functionality.</param>
        /// <returns>SQLite connection string pointing to data source file.</returns>
        /// <remarks>Uses <paramref name="configuration"/>["output"] for default output path and <paramref name="configuration"/>["database-name"] for data source name.</remarks>
        private static string ConstructSqliteConnectionString(IConfiguration configuration, IConsoleWrapper consoleWrapper)
        {
            var outputPath = configuration.GetValue<string>("output") ?? Directory.GetCurrentDirectory();
            var databaseName = configuration.GetValue<string>("database-name");

            if (string.IsNullOrEmpty(databaseName))
            {
                consoleWrapper.Write("Data source name (PhotoOrganizer.db): ");
                var dataSourceName = consoleWrapper.ReadLine();
                databaseName = string.IsNullOrEmpty(dataSourceName) ? "PhotoOrganizer.db" : dataSourceName;
            }
            var databasePath = Path.Combine(outputPath, databaseName);

            var sqliteBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = databasePath
            };

            return sqliteBuilder.ConnectionString;
        }
    }
}
