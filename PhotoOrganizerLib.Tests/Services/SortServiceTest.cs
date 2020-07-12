using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Moq;
using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Services;
using Xunit;

namespace PhotoOrganizerLib.Tests.Services.Tests
{
    public class SortServiceTest
    {
        [Fact]
        public void ConstructorTest_OutputDirectoriesExist()
        {
            // Mock the rename service and configuration
            var renameServiceMock = new Mock<IRenameService>();
            var configurationMock = new Mock<IConfiguration>();

            // Creates a temp directory for testing
            var outputPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

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

            // Mock the Configuration["output"] value to the temp directory
            // renameServiceMock.Setup(m => m.RenameFile(It.IsAny<string>(), It.IsAny<string>()));
            configurationMock.SetupGet(p => p[It.Is<string>(s => s == "output")])
                .Returns(outputPath);

            // Get the mocked objects
            var renameService = renameServiceMock.Object;
            var configuration = configurationMock.Object;

            // Construct the sort service and get the output directories structure
            var sortService = new SortService(configuration, renameService);

            var actualOutputDirectories = sortService.OutputDirectories;

            // Assert the actual dictionary keys are equal to expected dictionary keys
            Assert.All(expectedOutputDirectories.Keys,
                key => Assert.True(actualOutputDirectories.ContainsKey(key)));

            // Assert that each expected and actual values are both sub- and superset of eachother
            foreach (var key in expectedOutputDirectories.Keys)
            {
                Assert.Subset(expectedOutputDirectories[key], actualOutputDirectories[key]);
                Assert.Superset(expectedOutputDirectories[key], actualOutputDirectories[key]);
            }
        }
    }
}