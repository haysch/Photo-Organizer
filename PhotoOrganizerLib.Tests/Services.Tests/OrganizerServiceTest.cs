using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PhotoOrganizerLib.Data;
using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Services;
using PhotoOrganizerLib.Tests.Utils;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PhotoOrganizerLib.Tests.Services.Tests
{
    public class OrganizerServiceTest
    {
        private readonly DbContextOptions<PhotoContext> DbOptions = new DbContextOptionsBuilder<PhotoContext>()
            .UseSqlite(CreateInMemoryDatabase())
            .Options;

        private readonly ILogger<IOrganizerService> logger = Mock.Of<ILogger<IOrganizerService>>();

        // Helper method for creating InMemoryConfiguration
        private static IConfiguration CreateInMemoryConfiguration(string hashAlgorithm)
        {
            var memConfig = new Dictionary<string, string>
            {
                { "hash-algorithm", hashAlgorithm }
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(memConfig)
                .Build();
        }

        // Helper method for creating an in memory database
        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        [Fact]
        public async Task RunOrganizer_NoPhotos()
        {
            /// Description
            /// Test the organizer with no photos

            /// Expectation
            /// ISortService.SortPhoto is never called. Database is empty.

            // Mock ISortService
            var sortServiceMock = new Mock<ISortService>();

            // Setup SortPhoto method
            sortServiceMock.Setup(mock => mock.SortPhoto(It.IsAny<Photo>(), It.IsAny<string>()))
                .Verifiable();

            // Fetch mocked objects
            var sortService = sortServiceMock.Object;

            // Create IConfiguration
            var configuration = CreateInMemoryConfiguration("MD5");

            // Create workdirectory in Temp
            var tempDirectory = PathHelper.GetTemporaryDirectory();

            // Setup DbContextOptions
            using var dbContext = new PhotoContext(DbOptions);
            await dbContext.Database.EnsureDeletedAsync();

            // Init and call OrganizerService
            var organizerService = new OrganizerService(logger, configuration, sortService, dbContext);
            await organizerService.RunOrganizerAsync(tempDirectory);

            // Verify mock call
            sortServiceMock.Verify(mock => mock.SortPhoto(It.IsAny<Photo>(), It.IsAny<string>()), Times.Never);

            // Assert database is empty
            Assert.Empty(dbContext.Photos);
        }

        [Fact]
        public async Task RunOrganizer_FivePhotos()
        {
            /// Description
            /// Generates JPEG files and runs the organizer service

            /// Expectation
            /// The generated JPEG files are found, sorted and added to the database

            var files = 5;

            // Create temp JPEG files
            var tempDirectory = PathHelper.GetTemporaryDirectory();

            var filenames = new List<string>();
            for (var i = 0; i < files; i++)
            {
                // Generate faux JPEG files and save names
                var filepath = PathHelper.CreateImageFile(tempDirectory, ImageFormat.Jpeg);
                filenames.Add(Path.GetFileName(filepath));
            }

            // Mock ISortService
            var sortServiceMock = new Mock<ISortService>();

            // Setup SortPhoto method
            sortServiceMock.Setup(mock => mock.SortPhoto(It.IsAny<Photo>(), It.IsAny<string>()))
                .Verifiable();

            // Fetch mocked objects
            var sortService = sortServiceMock.Object;

            // Create IConfiguration
            var configuration = CreateInMemoryConfiguration("MD5");

            // Setup DbContextOptions
            using var dbContext = new PhotoContext(DbOptions);
            await dbContext.Database.EnsureDeletedAsync();

            // Init and call OrganizerService
            var organizerService = new OrganizerService(logger, configuration, sortService, dbContext);
            await organizerService.RunOrganizerAsync(tempDirectory);

            // Verify mock call
            sortServiceMock.Verify(mock => mock.SortPhoto(It.IsAny<Photo>(), It.IsAny<string>()), Times.Exactly(files));

            // Assert database is not empty and contains the generated files
            Assert.NotEmpty(dbContext.Photos);

            foreach (var fn in filenames)
            {
                Assert.NotNull(dbContext.Photos.Find(fn));
            }
        }
    }
}