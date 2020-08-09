using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PhotoOrganizerLib.Enums;
using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace PhotoOrganizerLib.Tests.Services.Tests
{
    public class SortServiceTest
    {
        private static readonly string UNKNOWN_DIRNAME = "unknown";
        private readonly ILogger<ISortService> logger = Mock.Of<ILogger<ISortService>>();

        private static IConfiguration CreateInMemoryConfiguration(string outputPath)
        {
            var memConfig = new Dictionary<string, string>
            {
                { "output", outputPath }
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(memConfig)
                .Build();
        }

        [Fact]
        public void ConstructorTest_OutputDirectoriesNotEmpty_And_UnknownDirectoryDoesNotExist()
        {
            /// Description:
            /// Tests whether it is possible to construct an dictionary that contains the existing yyyy/mm folders.

            /// Expectation:
            /// The dictionary only contains year entries with corresponding months.

            // Mock IRenameService
            var renameServiceMock = new Mock<IRenameService>();

            // Creates a temp directory for testing and a junk folder to mimic that other folders might exist
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(Path.Combine(outputPath, "junk"));

            // Generate random year and month limits used for testing
            var rand = new Random();
            var yearsLower = rand.Next(1970, 1976);
            var yearsUpper = rand.Next(yearsLower, 1981);

            var monthsLower = rand.Next(1, 7);
            var monthsUpper = rand.Next(monthsLower, 13);

            // Create the random yyyy/MM directories and
            // add to an dictionary
            var expectedOutputDirectories = new Dictionary<string, HashSet<string>>();
            for (var year = yearsLower; year <= yearsUpper; year++)
            {
                var yearString = year.ToString();
                var monthSet = new HashSet<string>();
                for (var month = monthsLower; month <= monthsUpper; month++)
                {
                    var monthString = month.ToString();

                    var yyyymm = Path.Combine(outputPath, yearString, monthString);
                    Directory.CreateDirectory(yyyymm);

                    monthSet.Add(monthString);
                }

                expectedOutputDirectories.Add(yearString, monthSet);
            }

            // Generate a temp IConfiguration object
            var configuration = CreateInMemoryConfiguration(outputPath);

            // Get the mocked objects
            var renameService = renameServiceMock.Object;

            // Construct the sort service and get the output directories structure
            var sortService = new SortService(logger, configuration, renameService);

            var actualOutputDirectories = sortService.OutputDirectories;

            // Assert that actual output directories key and value (year and months) are not empty
            Assert.NotEmpty(actualOutputDirectories);
            Assert.NotEmpty(actualOutputDirectories.Values);

            // Assert the actual dictionary keys are equal to expected dictionary keys
            Assert.All(expectedOutputDirectories.Keys,
                key => Assert.True(actualOutputDirectories.ContainsKey(key)));

            // Assert that each expected and actual values are both sub- and superset of eachother
            foreach (var key in expectedOutputDirectories.Keys)
            {
                Assert.Subset(expectedOutputDirectories[key], actualOutputDirectories[key]);
                Assert.Superset(expectedOutputDirectories[key], actualOutputDirectories[key]);
            }

            // Assert unknown directory does not exist
            var unknownDirectory = Path.Combine(outputPath, UNKNOWN_DIRNAME);
            Assert.False(Directory.Exists(unknownDirectory));
        }

        [Fact]
        public void Constructor_InvalidPath()
        {
            // Mock IRenameService
            var renameServiceMock = new Mock<IRenameService>();

            // Creates a temp directory for testing
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Generate a temp IConfiguration object
            var configuration = CreateInMemoryConfiguration(outputPath);

            // Get the mocked objects
            var renameService = renameServiceMock.Object;

            // Construct the sort service and get the output directories structure
            Assert.Throws<DirectoryNotFoundException>(() => new SortService(logger, configuration, renameService));
        }

        [Fact]
        public void SortDateTime_InvalidDateTimeName_SortAsUnknown()
        {
            /// Description:
            /// Sort a file with invalid datetime name to the unknown output folder.

            /// Expectation:
            /// Call renameServiceMock.RenameFile exactly ONCE!

            // Mock IRenameService
            var renameServiceMock = new Mock<IRenameService>();

            // Creates a temp directory for testing
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputPath);

            // Generate a temp IConfiguration object
            var configuration = CreateInMemoryConfiguration(outputPath);

            // Mock the RenameService.RenameFile to create file in unknown directory
            renameServiceMock.Setup(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()));

            // Get the mocked objects
            var renameService = renameServiceMock.Object;

            // Construct the sort service and get the output directories structure
            var sortService = new SortService(logger, configuration, renameService);

            sortService.SortDateTime("bob.jpg", "bob-the-builder");

            // Verify mock method called
            renameServiceMock.Verify(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            // Assert that unknown directory exists
            var unknownDirectoryPath = Path.Combine(outputPath, UNKNOWN_DIRNAME);
            Assert.True(Directory.Exists(unknownDirectoryPath));
        }

        [Fact]
        public void SortPhoto_ValidDateTimeName_yyyymmDirectoryNotExist()
        {
            /// Description
            /// Tests that it is possible to create year and month directories, if they do not exist.
            /// Also adds the newly created yyyy/mm directories as a new entry in the `OutputDirectories` dictionary

            /// Expectation
            /// Calls renameServiceMock.FindPhotoDateTime and renameServiceMock.RenameFile exactly ONCE!

            // Mock IRenameService
            var renameServiceMock = new Mock<IRenameService>();

            // Creates a temp directory for testing
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputPath);

            // Create temp photo
            var photoPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.Create(photoPath)
                .Close();

            var photo = new Photo(photoPath);

            var expectedDateTime = DateTime.Today.ToString("yyyyMMdd_HHmmss");

            // Generate a temp IConfiguration object
            var configuration = CreateInMemoryConfiguration(outputPath);

            // Mock the IRenameService.RenameFile to create file in unknown directory
            renameServiceMock.Setup(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()));

            // Mock the IRenameService.FindPhotoDateTime to return datetime name
            renameServiceMock.Setup(mock => mock.FindPhotoDateTime(It.IsAny<Photo>(), It.IsAny<string>()))
                .Returns(expectedDateTime);

            // Get the mocked objects
            var renameService = renameServiceMock.Object;

            // Construct the sort service and get the output directories structure
            var sortService = new SortService(logger, configuration, renameService);

            sortService.SortPhoto(photo);

            // Verify mock calls
            renameServiceMock.Verify(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            renameServiceMock.Verify(mock => mock.FindPhotoDateTime(It.IsAny<Photo>(), It.IsAny<string>()), Times.Once);

            // Assert that unknown directory does not exist
            var unknownDirectoryPath = Path.Combine(outputPath, UNKNOWN_DIRNAME);
            Assert.False(Directory.Exists(unknownDirectoryPath));
        }

        [Fact]
        public void SortPhoto_ValidDateTimeName_yyyymmDirectoryExist()
        {
            /// Description
            /// Tests that it is possible to rename and sort photo if the directories exist.

            /// Expectation
            /// Calls renameServiceMock.FindPhotoDateTime and renameServiceMock.RenameFile exactly ONCE!

            // Mock IRenameService
            var renameServiceMock = new Mock<IRenameService>();

            // Creates a temp directory for testing
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputPath);

            // Create temp photo
            var photoPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.Create(photoPath)
                .Close();

            var photo = new Photo(photoPath);

            var today = DateTime.Today;
            var yearString = today.Year.ToString();
            var monthString = today.Month.ToString("D2");
            var expectedDateTime = today.ToString("yyyyMMdd_HHmmss");
            var yyyymmPath = Path.Combine(outputPath, yearString, monthString);
            Directory.CreateDirectory(yyyymmPath);

            // Generate a temp IConfiguration object
            var configuration = CreateInMemoryConfiguration(outputPath);

            // Mock the IRenameService.RenameFile to create file in unknown directory
            renameServiceMock.Setup(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()));

            // Mock the IRenameService.FindPhotoDateTime to return datetime name
            renameServiceMock.Setup(mock => mock.FindPhotoDateTime(It.IsAny<Photo>(), It.IsAny<string>()))
                .Returns(expectedDateTime);

            // Get the mocked objects
            var renameService = renameServiceMock.Object;

            // Construct the sort service and get the output directories structure
            var sortService = new SortService(logger, configuration, renameService);

            sortService.SortPhoto(photo);

            // Verify mock calls
            renameServiceMock.Verify(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            renameServiceMock.Verify(mock => mock.FindPhotoDateTime(It.IsAny<Photo>(), It.IsAny<string>()), Times.Once);

            // Assert that unknown directory does not exist
            var unknownDirectoryPath = Path.Combine(outputPath, UNKNOWN_DIRNAME);
            Assert.False(Directory.Exists(unknownDirectoryPath));
        }

        [Fact]
        public void SortPhoto_ValidDateTimeName_yyyyDirectoryExist()
        {
            /// Description
            /// Tests that it is possible to create the month directory if they do not exist.
            /// Also adds the newly created month directory to the existing month set in the `OutputDirectories` dictionary.

            /// Expectation
            /// Calls renameServiceMock.FindPhotoDateTime and renameServiceMock.RenameFile exactly ONCE!

            // Mock IRenameService
            var renameServiceMock = new Mock<IRenameService>();

            // Creates a temp directory for testing
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputPath);

            // Create temp photo
            var photoPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.Create(photoPath)
                .Close();

            var photo = new Photo(photoPath);

            var today = DateTime.Today;
            var expectedDateTime = today.ToString("yyyyMMdd_HHmmss");
            var yyyyPath = Path.Combine(outputPath, today.Year.ToString());
            Directory.CreateDirectory(yyyyPath);

            // Generate a temp IConfiguration object
            var configuration = CreateInMemoryConfiguration(outputPath);

            // Mock the IRenameService.RenameFile to create file in unknown directory
            renameServiceMock.Setup(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()));

            // Mock the IRenameService.FindPhotoDateTime to return datetime name
            renameServiceMock.Setup(mock => mock.FindPhotoDateTime(It.IsAny<Photo>(), It.IsAny<string>()))
                .Returns(expectedDateTime);

            // Get the mocked objects
            var renameService = renameServiceMock.Object;

            // Construct the sort service and get the output directories structure
            var sortService = new SortService(logger, configuration, renameService);

            sortService.SortPhoto(photo);

            // Verify mock calls
            renameServiceMock.Verify(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            renameServiceMock.Verify(mock => mock.FindPhotoDateTime(It.IsAny<Photo>(), It.IsAny<string>()), Times.Once);

            // Assert that unknown directory does not exist
            var unknownDirectoryPath = Path.Combine(outputPath, UNKNOWN_DIRNAME);
            Assert.False(Directory.Exists(unknownDirectoryPath));
        }


        [Fact]
        public void SortPhoto_yyyymmDirectoryNotExist_CreateDirectory_RenameFile()
        {
            /// Description
            /// Test that it is possible to create directory that does not exist and then rename file.

            /// Expectation
            /// Call renameServiceMock.FindPhotoDateTime and renameServiceMock.RenameFile are called exactly ONCE

            // Mock IRenameService
            var renameServiceMock = new Mock<IRenameService>();

            // Creates a temp directory for testing
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputPath);

            // Create temp photo 
            var photoPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".abc");
            File.Create(photoPath)
                .Close();

            var photo = new Photo(photoPath);

            var today = DateTime.Today;
            var yearString = today.Year.ToString();
            var monthString = today.Month.ToString("D2");
            var expectedDateTime = today.ToString("yyyyMMdd_HHmmss");

            // Generate a temp IConfiguration object
            var configuration = CreateInMemoryConfiguration(outputPath);

            // Mock the IRenameService.RenameFile to create file in unknown directory
            renameServiceMock.Setup(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()));

            // Mock the IRenameService.FindPhotoDateTime to return datetime name
            renameServiceMock.Setup(mock => mock.FindPhotoDateTime(It.IsAny<Photo>(), It.IsAny<string>()))
                .Returns(expectedDateTime);

            // Get the mocked objects
            var renameService = renameServiceMock.Object;

            // Construct the sort service and get the output directories structure
            var sortService = new SortService(logger, configuration, renameService);

            // Assert that OutputDirectories does NOT contain new entry
            Assert.False(sortService.OutputDirectories.ContainsKey(yearString));
            Assert.False(sortService.OutputDirectories.TryGetValue(yearString, out var monthSetBefore) && monthSetBefore.Contains(monthString));

            sortService.SortPhoto(photo);

            // Assert that OutputDirectories contains new entry
            Assert.True(sortService.OutputDirectories.ContainsKey(yearString));
            Assert.True(sortService.OutputDirectories.TryGetValue(yearString, out var monthSetAfter) && monthSetAfter.Contains(monthString));

            // Verify mock calls
            renameServiceMock.Verify(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            renameServiceMock.Verify(mock => mock.FindPhotoDateTime(It.IsAny<Photo>(), It.IsAny<string>()), Times.Once);

            // Assert that unknown directory does not exist
            var unknownDirectoryPath = Path.Combine(outputPath, UNKNOWN_DIRNAME);
            Assert.False(Directory.Exists(unknownDirectoryPath));
        }

        [Fact]
        public void Constructor_OutputPathNotDefined()
        {
            /// Description
            /// Test that the output path is set to current directory

            /// Expectation
            /// An unknown directory is created in the current directory

            // Mock IRenameService
            var renameServiceMock = new Mock<IRenameService>();

            // Get the mocked objects
            var renameService = renameServiceMock.Object;

            var configuration = new ConfigurationBuilder().Build();

            // Construct the sort service with no output path defined
            new SortService(logger, configuration, renameService);

            // Assert unknown directory is created in current directory
            var unknownDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), UNKNOWN_DIRNAME);
            Assert.True(Directory.Exists(unknownDirectoryPath));
        }

        [Fact]
        public void SortUnknownFile_NoneRenameType()
        {
            /// Description
            /// Test that the unknown directory is not created when RenameType is None.

            /// Expectation
            /// Unknown directory does not exist

            // Mock IRenameService
            var renameServiceMock = new Mock<IRenameService>();

            // Mock RenameType
            renameServiceMock.SetupGet(mock => mock.RenameType)
                .Returns(RenameType.None)
                .Verifiable();

            // Creates a temp directory for testing
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputPath);

            // Generate a temp IConfiguration object
            var configuration = CreateInMemoryConfiguration(outputPath);

            // Get the mocked objects
            var renameService = renameServiceMock.Object;

            // Construct the sort service and get the output directories structure
            var sortService = new SortService(logger, configuration, renameService);

            sortService.SortDateTime("bob.jpg", "bob-the-builder");

            // Verify RenameType is called once
            renameServiceMock.VerifyGet(mock => mock.RenameType, Times.Once);

            // Assert unknown directory is not created
            var unknownDirectoryPath = Path.Combine(outputPath, UNKNOWN_DIRNAME);
            Assert.False(Directory.Exists(unknownDirectoryPath));
        }
    }
}