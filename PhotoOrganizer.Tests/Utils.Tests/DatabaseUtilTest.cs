using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Npgsql;
using PhotoOrganizer.Enums;
using PhotoOrganizer.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace PhotoOrganizer.Tests.Utils.Tests
{
    public class DatabaseUtilTest
    {
        private IConfiguration SetupInMemoryConfiguration(Dictionary<string, string> memoryCollection)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(memoryCollection)
                .Build();
        }

        [Fact]
        public void InvalidInputConnectionString()
        {
            var connectionString = Guid.NewGuid().ToString();
            var memoryCollection = new Dictionary<string, string>
            {
                { "connectionstring", connectionString }
            };

            var configuration = SetupInMemoryConfiguration(memoryCollection);
            var consoleWrapper = new TestConsoleWrapper(null, null);

            var actualConnectionString = DatabaseUtil.ConstructDbConnectionString(configuration, DatabaseFlag.MySQL, consoleWrapper, false);

            Assert.Null(actualConnectionString);
        }

        [Fact]
        public void ValidPostgreSQLConnectionString()
        {
            var connectionString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=postgres;";
            var memoryCollection = new Dictionary<string, string>
            {
                { "connectionstring", connectionString }
            };

            var configuration = SetupInMemoryConfiguration(memoryCollection);
            var consoleWrapper = new TestConsoleWrapper(null, null);

            var actualConnectionString = DatabaseUtil.ConstructDbConnectionString(configuration, DatabaseFlag.PostgreSQL, consoleWrapper, false);

            Assert.NotNull(actualConnectionString);
            Assert.Equal(connectionString, actualConnectionString);
        }

        [Fact]
        public void NullConnectionString_Noninteractive_Throw()
        {
            var memoryCollection = new Dictionary<string, string>
            {
                { "connectionstring", null }
            };

            var configuration = SetupInMemoryConfiguration(memoryCollection);
            var consoleWrapper = new TestConsoleWrapper(null, null);

            foreach (DatabaseFlag dbFlag in Enum.GetValues(typeof(DatabaseFlag)))
            {
                Assert.Throws<ArgumentException>(() => DatabaseUtil.ConstructDbConnectionString(configuration, dbFlag, consoleWrapper, false));
            }
        }

        [Fact]
        public void GenerateSqliteConnectionString_Interactive_WithInput()
        {
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var memoryCollection = new Dictionary<string, string>
            {
                { "output", outputPath }
            };
            var configuration = SetupInMemoryConfiguration(memoryCollection);

            var databaseName = "TestPhotos.db";
            var lines = new List<string> { databaseName };
            var consoleWrapper = new TestConsoleWrapper(null, lines);

            var databasePath = Path.Combine(outputPath, databaseName);
            var expectedConnectionString = new SqliteConnectionStringBuilder
            {
                DataSource = databasePath
            }.ConnectionString;

            var actualConnectionString = DatabaseUtil.ConstructDbConnectionString(configuration, DatabaseFlag.SQLite, consoleWrapper, true);

            Assert.Equal(expectedConnectionString, actualConnectionString);
        }

        [Fact]
        public void GenerateSqliteConnectionString_Interactive_NoInput()
        {
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var memoryCollection = new Dictionary<string, string>
            {
                { "output", outputPath }
            };
            var configuration = SetupInMemoryConfiguration(memoryCollection);

            var databaseName = "PhotoOrganizer.db";
            var consoleWrapper = new TestConsoleWrapper(null, null);

            var databasePath = Path.Combine(outputPath, databaseName);
            var expectedConnectionString = new SqliteConnectionStringBuilder
            {
                DataSource = databasePath
            }.ConnectionString;

            var actualConnectionString = DatabaseUtil.ConstructDbConnectionString(configuration, DatabaseFlag.SQLite, consoleWrapper, true);

            Assert.Equal(expectedConnectionString, actualConnectionString);
        }

        [Fact]
        public void GenerateSqliteConnectionString_Interactive_PredefinedDbName()
        {
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var databaseName = "bob-the-builder.db";
            var memoryCollection = new Dictionary<string, string>
            {
                { "output", outputPath },
                { "database-name", databaseName }
            };
            var configuration = SetupInMemoryConfiguration(memoryCollection);

            var consoleWrapper = new TestConsoleWrapper(null, null);

            var databasePath = Path.Combine(outputPath, databaseName);
            var expectedConnectionString = new SqliteConnectionStringBuilder
            {
                DataSource = databasePath
            }.ConnectionString;

            var actualConnectionString = DatabaseUtil.ConstructDbConnectionString(configuration, DatabaseFlag.SQLite, consoleWrapper, true);

            Assert.Equal(expectedConnectionString, actualConnectionString);
        }

        [Fact]
        public void GenerateMySQLConnectionString_Interactive()
        {
            var memoryCollection = new Dictionary<string, string>();
            var configuration = SetupInMemoryConfiguration(memoryCollection);

            // input variables
            var userId = "mysql";
            var password = "My/Sql";
            var server = "localhost";
            uint port = 3306;
            var database = "Photos.db";

            var consoleKeyInput = new List<ConsoleKeyInfo> // Password
            {
                new ConsoleKeyInfo('\u004D', ConsoleKey.M, true, false, false),   // M
                new ConsoleKeyInfo('\u0079', ConsoleKey.Y, false, false, false),  // y
                new ConsoleKeyInfo('\u002F', ConsoleKey.D4, true, false, false),  // '/'
                new ConsoleKeyInfo('\u0053', ConsoleKey.S, true, false, false),   // S
                new ConsoleKeyInfo('\u0071', ConsoleKey.Q, false, false, false),  // q
                new ConsoleKeyInfo('\u006C', ConsoleKey.L, false, false, false),  // l
                new ConsoleKeyInfo((char)ConsoleKey.Enter, ConsoleKey.Enter, false, false, false) // Enter
            };
            var consoleLineInput = new List<string> { userId, server, port.ToString(), database };
            var consoleWrapper = new TestConsoleWrapper(consoleKeyInput, consoleLineInput);

            var mysqlCsBuilder = new MySqlConnectionStringBuilder
            {
                UserID = userId,
                Password = password,
                Server = server,
                Port = port,
                Database = database
            };

            var expectedConnectionString = mysqlCsBuilder.ConnectionString;

            var actualConnectionString = DatabaseUtil.ConstructDbConnectionString(configuration, DatabaseFlag.MySQL, consoleWrapper, true);

            Assert.Equal(expectedConnectionString, actualConnectionString);
        }

        [Fact]
        public void GeneratePostgreSQLConnectionString_Interactive()
        {
            var memoryCollection = new Dictionary<string, string>();
            var configuration = SetupInMemoryConfiguration(memoryCollection);

            // input variables
            var username = "postgres";
            var password = "posgres";
            var host = "localhost";
            var port = 3306;
            var database = "Photos.db";

            var consoleKeyInput = new List<ConsoleKeyInfo> // Password
            {
                new ConsoleKeyInfo('p', ConsoleKey.P, false, false, false),  // p
                new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false),  // o
                new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false),  // s
                new ConsoleKeyInfo('t', ConsoleKey.T, false, false, false),  // t
                new ConsoleKeyInfo((char)ConsoleKey.Backspace, ConsoleKey.Backspace, false, false, false), // backspace (delete t)
                new ConsoleKeyInfo('g', ConsoleKey.G, false, false, false),  // g
                new ConsoleKeyInfo('r', ConsoleKey.R, false, false, false),  // r
                new ConsoleKeyInfo('e', ConsoleKey.E, false, false, false),  // e
                new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false),  // s
                new ConsoleKeyInfo((char)ConsoleKey.Enter, ConsoleKey.Enter, false, false, false) // Enter
            };
            var consoleLineInput = new List<string> { username, host, port.ToString(), database };
            var consoleWrapper = new TestConsoleWrapper(consoleKeyInput, consoleLineInput);

            var npgsqlCsBuilder = new NpgsqlConnectionStringBuilder
            {
                Username = username,
                Password = password,
                Host = host,
                Port = port,
                Database = database
            };

            var expectedConnectionString = npgsqlCsBuilder.ConnectionString;

            var actualConnectionString = DatabaseUtil.ConstructDbConnectionString(configuration, DatabaseFlag.PostgreSQL, consoleWrapper, true);

            Assert.Equal(expectedConnectionString, actualConnectionString);
        }

        [Fact]
        public void GeneratePostgreSQLConnectionString_Interactive_InvalidPort()
        {
            var memoryCollection = new Dictionary<string, string>();
            var configuration = SetupInMemoryConfiguration(memoryCollection);

            // input variables
            var username = "postgres";
            var password = "posgres";
            var host = "localhost";
            var portInvalid = "abc";
            var portExpected = 3306;
            var database = "Photos.db";

            var consoleKeyInput = new List<ConsoleKeyInfo> // Password
            {
                new ConsoleKeyInfo('p', ConsoleKey.P, false, false, false),  // p
                new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false),  // o
                new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false),  // s
                new ConsoleKeyInfo('t', ConsoleKey.T, false, false, false),  // t
                new ConsoleKeyInfo((char)ConsoleKey.Backspace, ConsoleKey.Backspace, false, false, false), // backspace (delete t)
                new ConsoleKeyInfo('g', ConsoleKey.G, false, false, false),  // g
                new ConsoleKeyInfo('r', ConsoleKey.R, false, false, false),  // r
                new ConsoleKeyInfo('e', ConsoleKey.E, false, false, false),  // e
                new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false),  // s
                new ConsoleKeyInfo((char)ConsoleKey.Enter, ConsoleKey.Enter, false, false, false) // Enter
            };
            var consoleLineInput = new List<string> { username, host, portInvalid, portExpected.ToString(), database };
            var consoleWrapper = new TestConsoleWrapper(consoleKeyInput, consoleLineInput);

            var npgsqlCsBuilder = new NpgsqlConnectionStringBuilder
            {
                Username = username,
                Password = password,
                Host = host,
                Port = portExpected,
                Database = database
            };

            var expectedConnectionString = npgsqlCsBuilder.ConnectionString;

            var actualConnectionString = DatabaseUtil.ConstructDbConnectionString(configuration, DatabaseFlag.PostgreSQL, consoleWrapper, true);

            Assert.Equal(expectedConnectionString, actualConnectionString);
        }

        [Fact]
        public void GenerateSQLServerConnectionString_Interactive()
        {
            var memoryCollection = new Dictionary<string, string>();
            var configuration = SetupInMemoryConfiguration(memoryCollection);

            // input variables
            var userId = "sqlSrv";
            var password = "sqlSrv";
            var host = "localhost";
            var port = 1433;
            var database = "Photos.db";

            var consoleKeyInput = new List<ConsoleKeyInfo> // Password
            {
                new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false),  // s
                new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false),  // q
                new ConsoleKeyInfo('l', ConsoleKey.L, false, false, false),  // l
                new ConsoleKeyInfo('S', ConsoleKey.S, true, false, false),   // S
                new ConsoleKeyInfo('r', ConsoleKey.R, false, false, false),  // r
                new ConsoleKeyInfo('v', ConsoleKey.V, false, false, false),  // v
                new ConsoleKeyInfo((char)ConsoleKey.Enter, ConsoleKey.Enter, false, false, false) // Enter
            };
            var consoleLineInput = new List<string> { userId, host, port.ToString(), database };
            var consoleWrapper = new TestConsoleWrapper(consoleKeyInput, consoleLineInput);

            var npgsqlCsBuilder = new SqlConnectionStringBuilder
            {
                UserID = userId,
                Password = password,
                DataSource = $"{host},{port}",
                InitialCatalog = database
            };

            var expectedConnectionString = npgsqlCsBuilder.ConnectionString;

            var actualConnectionString = DatabaseUtil.ConstructDbConnectionString(configuration, DatabaseFlag.SQLServer, consoleWrapper, true);

            Assert.Equal(expectedConnectionString, actualConnectionString);
        }
    }
}
