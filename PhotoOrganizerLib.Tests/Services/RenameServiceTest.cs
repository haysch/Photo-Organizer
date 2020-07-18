using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Services;
using Xunit;

namespace PhotoOrganizerLib.Tests.Services
{
    public class RenameServiceTest
    {
        /// Helper method for creating an IConfiguration using InMemoryCollection
        private static IConfiguration CreateInMemoryConfiguration(string type)
        {
            var memConfig = new Dictionary<string, string>
            {
                { "renameType", type }
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

                new RenameService(configuration);
            }
        }

        [Fact]
        public void Constructor_UnsupportedRenameTypeString()
        {
            var invalidRtString = "bob-the-builder";
            var configuration = CreateInMemoryConfiguration(invalidRtString);

            Assert.Throws<ArgumentException>(() => new RenameService(configuration));
        }

        [Fact]
        public void Constructor_NullRenameType()
        {
            var configuration = CreateInMemoryConfiguration(null);

            Assert.Throws<ArgumentException>(() => new RenameService(configuration));
        }

        [Fact]
        public void FindPhotoDateTime_ValidDateTimeString_And_Format()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService and Photo
            var renameService = new RenameService(configuration);
            var photo = new Photo("");

            // Test that FindPhotoDateTime finds datetime with correct format
            var dateTimeToday = DateTime.Today;
            var format = "yyyyMMdd_HHmmss";
            Assert.True(photo.TryAddMetadata("DateTimeOriginal", dateTimeToday.ToString()));

            var expectedDateTimeString = dateTimeToday.ToString(format);

            var actualDateTimeString = renameService.FindPhotoDateTime(photo, format);

            Assert.Equal(expectedDateTimeString, actualDateTimeString);
        }

        [Fact]
        public void FindPhotoDateTime_InvalidDateTimeString()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService and Photo
            var renameService = new RenameService(configuration);
            var photo = new Photo("");

            // Test that FindPhotoDateTime finds datetime with correct format
            var dateTimeToday = "bob-the-builder";
            var format = "yyyyMMdd_HHmmss";
            Assert.True(photo.TryAddMetadata("DateTimeOriginal", dateTimeToday.ToString()));

            var actualDateTimeString = renameService.FindPhotoDateTime(photo, format);

            Assert.Empty(actualDateTimeString);
        }

        [Fact]
        public void FindPhotoDateTime_ValidDateTimeString_InvalidFormat()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService and Photo
            var renameService = new RenameService(configuration);
            var photo = new Photo("");

            // Test that FindPhotoDateTime finds datetime with correct format
            var dateTimeToday = DateTime.Today;
            var format = "h";
            Assert.True(photo.TryAddMetadata("DateTimeOriginal", dateTimeToday.ToString()));

            Assert.Throws<FormatException>(() => renameService.FindPhotoDateTime(photo, format));
        }

        [Fact]
        public void RenameFile_Copy_ExistingSource()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(configuration);

            // Create source file
            var sourcePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var fs = File.Create(sourcePath);

            // Remember to close FileStream
            fs.Close();

            // Copy source to target
            var targetPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(sourcePath, targetPath);

            Assert.True(File.Exists(sourcePath));
            Assert.True(File.Exists(targetPath));
        }

        [Fact]
        public void RenameFile_Move_ExistingSource()
        {
            var renameString = "move";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(configuration);

            // Create source file
            var sourcePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var fs = File.Create(sourcePath);

            // Remember to close FileStream
            fs.Close();

            // Move source to target
            var targetPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(sourcePath, targetPath);

            Assert.False(File.Exists(sourcePath));
            Assert.True(File.Exists(targetPath));
        }

        [Fact]
        public void RenameFile_None_ExistingSource()
        {
            var renameString = "none";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(configuration);

            // Create source file
            var sourcePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var fs = File.Create(sourcePath);

            // Remember to close FileStream
            fs.Close();

            // Do nothing ¯\_(ツ)_/¯
            var targetPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(sourcePath, targetPath);

            Assert.True(File.Exists(sourcePath));
            Assert.False(File.Exists(targetPath));
        }

        [Fact]
        public void RenameFile_FileNotFoundException_NoFilesExist()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(configuration);

            // Create source path, but no file
            var sourcePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // "Copy" source to target
            var targetPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(sourcePath, targetPath);

            Assert.False(File.Exists(sourcePath));
            Assert.False(File.Exists(targetPath));
        }

        [Fact]
        public void RenameFile_DirectoryNotFoundException_NoFilesExist()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(configuration);

            // Create source path, but no file
            var sourcePath = Path.Combine(Path.GetRandomFileName(), Path.GetRandomFileName());

            // "Copy" source to target
            var targetPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(sourcePath, targetPath);

            Assert.False(File.Exists(sourcePath));
            Assert.False(File.Exists(targetPath));
        }

        [Fact]
        public void RenameFile_IOException_ProcessHoldingFileAccess_SourceExists()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(configuration);

            // Create source path and file, keep file open
            var sourcePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            using (var fs = File.Create(sourcePath))
            {
                // "Copy" source to target
                var targetPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                renameService.RenameFile(sourcePath, targetPath);

                Assert.True(File.Exists(sourcePath));
                Assert.False(File.Exists(targetPath));
            }
        }

        [Fact]
        public void RenameFile_ArgumentNullException_NoFilesExist()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(configuration);

            // "Copy" source to target
            var targetPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(null, targetPath);

            Assert.False(File.Exists(targetPath));
        }

        [Fact]
        public void RenameFile_ArgumentException_EmptySourceName_NoFilesExist()
        {
            var renameString = "copy";
            var configuration = CreateInMemoryConfiguration(renameString);

            // Setup RenameService
            var renameService = new RenameService(configuration);

            // Create source path, but no file
            var sourcePath = string.Empty;

            // "Copy" source to target
            var targetPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            renameService.RenameFile(sourcePath, targetPath);

            Assert.False(File.Exists(sourcePath));
            Assert.False(File.Exists(targetPath));
        }
    }
}