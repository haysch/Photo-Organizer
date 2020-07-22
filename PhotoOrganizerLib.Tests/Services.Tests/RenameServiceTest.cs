using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace PhotoOrganizerLib.Tests.Services.Tests
{
    public class RenameServiceTest
    {
        private readonly ILogger<IRenameService> logger = Mock.Of<ILogger<IRenameService>>();

        /// Helper method for creating an IConfiguration using InMemoryCollection
        private static IConfiguration CreateInMemoryConfiguration(string type)
        {
            var memConfig = new Dictionary<string, string>
            {
                { "rename-type", type }
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(memConfig)
                .Build();
        }

        [Fact]
        public void Constructor_SupportedRenameTypeString()
        {
            var renameTypeList = new List<string> { "copy", "move", "none" };

            foreach (var rt in renameTypeList)
            {
                var configuration = CreateInMemoryConfiguration(rt);

                new RenameService(logger, configuration);
            }
        }

        [Fact]
        public void Constructor_UnsupportedRenameTypeString()
        {
            var invalidRtString = "bob-the-builder";
            var configuration = CreateInMemoryConfiguration(invalidRtString);

            new RenameService(logger, configuration);
        }

        [Fact]
        public void Constructor_NullRenameType()
        {
            var configuration = CreateInMemoryConfiguration(null);

            new RenameService(logger, configuration);
        }

        [Fact]
        public void FindPhotoDateTime_ValidDateTime_And_Format()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService and Photo
            var renameService = new RenameService(logger, configuration);
            var photo = new Photo("");

            // Test that FindPhotoDateTime finds datetime with correct format
            var dateTimeToday = DateTime.Today;
            var format = "yyyyMMdd_HHmmss";
            photo.DateTimeOriginal = dateTimeToday;

            var expectedDateTimeString = dateTimeToday.ToString(format);

            var actualDateTimeString = renameService.FindPhotoDateTime(photo, format);

            Assert.Equal(expectedDateTimeString, actualDateTimeString);
        }

        [Fact]
        public void FindPhotoDateTime_InvalidDateTime()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService and Photo
            var renameService = new RenameService(logger, configuration);
            var photo = new Photo("");

            // Test that FindPhotoDateTime finds datetime with correct format
            var format = "yyyyMMdd_HHmmss";
            photo.DateTimeOriginal = null;

            var actualDateTimeString = renameService.FindPhotoDateTime(photo, format);

            Assert.Empty(actualDateTimeString);
        }

        [Fact]
        public void FindPhotoDateTime_ValidDateTimeString_InvalidFormat()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService and Photo
            var renameService = new RenameService(logger, configuration);
            var photo = new Photo("");

            // Test that FindPhotoDateTime finds datetime with correct format
            var dateTimeToday = DateTime.Today;
            var format = "h";
            photo.DateTimeOriginal = dateTimeToday;

            Assert.Throws<FormatException>(() => renameService.FindPhotoDateTime(photo, format));
        }

        [Fact]
        public void RenameFile_Copy_ExistingSource()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(logger, configuration);

            // Create source file
            var sourcePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.Create(sourcePath)
                .Close();

            // Copy source to target
            var destPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(sourcePath, destPath);

            Assert.True(File.Exists(sourcePath));
            Assert.True(File.Exists(destPath));
        }

        [Fact]
        public void RenameFile_Move_ExistingSource()
        {
            var renameString = "move";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(logger, configuration);

            // Create source file
            var sourcePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.Create(sourcePath)
                .Close();

            // Move source to target
            var destPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(sourcePath, destPath);

            Assert.False(File.Exists(sourcePath));
            Assert.True(File.Exists(destPath));
        }

        [Fact]
        public void RenameFile_None_ExistingSource()
        {
            var renameString = "none";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(logger, configuration);

            // Create source file
            var sourcePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.Create(sourcePath)
                .Close();

            // Do nothing ¯\_(ツ)_/¯
            var destPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(sourcePath, destPath);

            Assert.True(File.Exists(sourcePath));
            Assert.False(File.Exists(destPath));
        }

        [Fact]
        public void RenameFile_FileNotFoundException_NoFilesExist()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(logger, configuration);

            // Create source path, but no file
            var sourcePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // "Copy" source to target
            var destPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            renameService.RenameFile(sourcePath, destPath);

            Assert.False(File.Exists(sourcePath));
            Assert.False(File.Exists(destPath));
        }

        [Fact]
        public void RenameFile_DirectoryNotFoundException_NoFilesExist()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(logger, configuration);

            // Create source path, but no file
            var sourcePath = Path.Combine(Path.GetRandomFileName(), Path.GetRandomFileName());

            // "Copy" source to target
            var destPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(sourcePath, destPath);

            Assert.False(File.Exists(sourcePath));
            Assert.False(File.Exists(destPath));
        }

        [Fact]
        public void RenameFile_IOException_ProcessHoldingFileAccess_SourceExists()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(logger, configuration);

            // Create source path and file, keep file open
            var sourcePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.Create(sourcePath);

            // "Copy" source to target
            var destPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(sourcePath, destPath);

            Assert.True(File.Exists(sourcePath));
            Assert.False(File.Exists(destPath));
        }

        [Fact]
        public void RenameFile_ArgumentNullException_SourcePathNull()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(logger, configuration);

            // "Copy" source to target
            var destPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(null, destPath);

            Assert.False(File.Exists(destPath));
        }

        [Fact]
        public void RenameFile_ArgumentNullException_DestPathNull()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(logger, configuration);

            // "Copy" source to target
            var sourcePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.Create(sourcePath)
                .Close();

            renameService.RenameFile(sourcePath, null);

            Assert.True(File.Exists(sourcePath));
        }

        [Fact]
        public void RenameFile_ArgumentException_EmptySourceName_NoFilesExist()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(logger, configuration);

            // Create source path, but no file
            var sourcePath = string.Empty;

            // "Copy" source to target
            var destPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(sourcePath, destPath);

            Assert.False(File.Exists(sourcePath));
            Assert.False(File.Exists(destPath));
        }
    }
}