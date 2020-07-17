using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Moq;
using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Services;
using Xunit;

namespace PhotoOrganizerLib.Tests.Services.Tests
{
    public class SortServiceTest
    {
        private static readonly string UNKNOWN_DIRNAME = "unknown";

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
        public void ConstructorTest_OutputDirectoriesNotEmpty_And_UnknownDirectoryExists()
        {
            /// Description:
            /// Tests whether it is possible to construct an dictionary that contains the existing yyyy/mm folders.
            
            /// Expectation:
            /// The dictionary only contains year entries with corresponding months.

            // Mock the rename service and configuration
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
            var sortService = new SortService(configuration, renameService);

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

            // Assert unknown directory exists
            var unknownDirectory = Path.Combine(outputPath, UNKNOWN_DIRNAME);
            Assert.True(Directory.Exists(unknownDirectory));
        }

        [Fact]
        public void Constructor_InvalidPath()
        {
            // Mock the rename service and configuration
            var renameServiceMock = new Mock<IRenameService>();

            // Creates a temp directory for testing
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Generate a temp IConfiguration object
            var configuration = CreateInMemoryConfiguration(outputPath);

            // Get the mocked objects
            var renameService = renameServiceMock.Object;

            // Construct the sort service and get the output directories structure
            Assert.Throws<DirectoryNotFoundException>(() => new SortService(configuration, renameService));
        }

        [Fact]
        public void SortDateTime_InvalidDateTimeName_SortAsUnknown()
        {
            /// Description:
            /// Sort a file with invalid datetime name to the unknown output folder.

            /// Expectation:
            /// Call renameServiceMock.RenameFile exactly ONCE!

            // Mock the rename service and configuration
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
            var sortService = new SortService(configuration, renameService);

            sortService.SortDateTime("bob.jpg", "bob-the-builder");

            renameServiceMock.Verify(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void SortPhoto_ValidDateTimeName_yyyymmDirectoryNotExist()
        {
            /// Description
            /// Tests that it is possible to create year and month directories, if they do not exist.
            /// Also adds the newly created yyyy/mm directories as a new entry in the `OutputDirectories` dictionary

            /// Expectation
            /// Calls renameServiceMock.FindPhotoDateTime and renameServiceMock.RenameFile exactly ONCE!

            // Mock the rename service and configuration
            var renameServiceMock = new Mock<IRenameService>();

            // Creates a temp directory for testing
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputPath);

            // Create temp photo
            var photoPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var fs = File.Create(photoPath);
            fs.Close();

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
            var sortService = new SortService(configuration, renameService);

            sortService.SortPhoto(photo);

            // Verify mock calls
            renameServiceMock.Verify(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            renameServiceMock.Verify(mock => mock.FindPhotoDateTime(It.IsAny<Photo>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void SortPhoto_ValidDateTimeName_yyyymmDirectoryExist()
        {
            /// Description
            /// Tests that it is possible to rename and sort photo if the directories exist.

            /// Expectation
            /// Calls renameServiceMock.FindPhotoDateTime and renameServiceMock.RenameFile exactly ONCE!

            // Mock the rename service and configuration
            var renameServiceMock = new Mock<IRenameService>();

            // Creates a temp directory for testing
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputPath);

            // Create temp photo
            var photoPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var fs = File.Create(photoPath);
            fs.Close();

            var photo = new Photo(photoPath);

            var today = DateTime.Today;
            var expectedDateTime = today.ToString("yyyyMMdd_HHmmss");
            var yyyymmPath = Path.Combine(outputPath, today.Year.ToString(), today.Month.ToString());
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
            var sortService = new SortService(configuration, renameService);

            sortService.SortPhoto(photo);

            // Verify mock calls
            renameServiceMock.Verify(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            renameServiceMock.Verify(mock => mock.FindPhotoDateTime(It.IsAny<Photo>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void SortPhoto_ValidDateTimeName_yyyyDirectoryExist()
        {
            /// Description
            /// Tests that it is possible to create the month directory if they do not exist.
            /// Also adds the newly created month directory to the existing month set in the `OutputDirectories` dictionary.

            /// Expectation
            /// Calls renameServiceMock.FindPhotoDateTime and renameServiceMock.RenameFile exactly ONCE!

            // Mock the rename service and configuration
            var renameServiceMock = new Mock<IRenameService>();

            // Creates a temp directory for testing
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputPath);

            // Create temp photo
            var photoPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var fs = File.Create(photoPath);
            fs.Close();

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
            var sortService = new SortService(configuration, renameService);

            sortService.SortPhoto(photo);

            // Verify mock calls
            renameServiceMock.Verify(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            renameServiceMock.Verify(mock => mock.FindPhotoDateTime(It.IsAny<Photo>(), It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public void SortPhoto_CannotCreateDirectory_yyyymmDirectoryNotExist_DoNotRename()
        {
            /// Description
            /// Tests that it is not possible to rename file if yyyyMM folder does not exist and
            /// the source path contains an invalid character (':') because Directory.CreateDirectory throws an exception

            /// Expectation
            /// Call renameServiceMock.FindPhotoDateTime exactly ONCE,
            /// but NEVER call renameServiceMock.RenameFile

            // Mock the rename service and configuration
            var renameServiceMock = new Mock<IRenameService>();

            // Creates a temp directory for testing
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputPath);

            // Create temp photo with file extension that contains ':'
            var photoPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".ab:c");
            var fs = File.Create(photoPath);
            fs.Close();
            
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
            var sortService = new SortService(configuration, renameService);

            sortService.SortPhoto(photo);

            // Verify mock calls
            renameServiceMock.Verify(mock => mock.RenameFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            renameServiceMock.Verify(mock => mock.FindPhotoDateTime(It.IsAny<Photo>(), It.IsAny<string>()), Times.Once);
        }
    }
}