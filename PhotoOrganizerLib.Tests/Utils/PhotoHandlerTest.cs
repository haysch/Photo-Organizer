using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Utils;
using Xunit;

namespace PhotoOrganizerLib.Tests.Utils
{
    public class PhotoHandlerTest
    {
        private static readonly byte[] JpegFormat = new byte[] { 0xff, 0xd8 };

        // Max directory depth used for testing
        private static readonly int MAXDEPTH = 10;

        [Fact]
        public async Task FindPhotosAsync_InvalidPathAsync()
        {
            var path = @"BlahBlahBuh";
            var photoList = new List<Photo>();
            await foreach (var photo in PhotoHandler.FindPhotosAsync(path))
                photoList.Add(photo);

            Assert.Empty(photoList);
        }

        [Fact]
        public async Task FindPhotosAsync_FlatDirectory_SingleFile_UnknownFormatAsync()
        {
            var tempDirectory = GetTemporaryDirectory();
            CreateTmpFile(tempDirectory); // create tmp file with unsupported filetype

            var photoList = new List<Photo>();
            await foreach (var photo in PhotoHandler.FindPhotosAsync(tempDirectory))
                photoList.Add(photo);

            Assert.Empty(photoList);
        }

        [Fact]
        public async Task FindPhotosAsync_FlatDirectory_SingleFile_JpegFormatAsync()
        {
            var tempDirectory = GetTemporaryDirectory();
            var tempFileName = CreateTmpFile(tempDirectory, JpegFormat);
            var expectedPath = Path.Combine(tempDirectory, tempFileName);

            var photoList = new List<Photo>();
            await foreach (var p in PhotoHandler.FindPhotosAsync(tempDirectory))
                photoList.Add(p);

            var photo = photoList.FirstOrDefault();
            var actualPath = photo.AbsoluteFilePath;

            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public async Task FindPhotosAsync_FlatDirectory_MultipleFiles_JpegFormatAsync()
        {
            var tempDirectory = GetTemporaryDirectory();

            // Creates the expected paths set from the file creation
            var expectedPaths = new HashSet<string>();
            for (var i = 0; i < 10; i++)
            {
                var path = Path.Combine(tempDirectory, CreateTmpFile(tempDirectory, JpegFormat));
                expectedPaths.Add(path);
            }

            // Creates set containing the actual paths
            var photoList = new List<Photo>();
            await foreach (var p in PhotoHandler.FindPhotosAsync(tempDirectory))
                photoList.Add(p);

            var actualPaths = new HashSet<string>();
            foreach (var photo in photoList)
            {
                actualPaths.Add(photo.AbsoluteFilePath);
            }

            // Assert the two sets are equal
            Assert.Subset(expectedPaths, actualPaths);
            Assert.Superset(expectedPaths, actualPaths);
        }

        [Fact]
        public async Task FindPhotosAsync_TwoLevelDirectory_SingleFile_JpegFormatAsync()
        {
            // Create deeper directory structure
            var tempDirectory = GetTemporaryDirectory();
            var newDirectory = Path.Combine(tempDirectory, Path.GetRandomFileName());
            Directory.CreateDirectory(newDirectory);

            // Find expected path from the file creation
            var tempFileName = CreateTmpFile(newDirectory, JpegFormat);
            var expectedPath = Path.Combine(newDirectory, tempFileName);

            // Find actual path
            var photoList = new List<Photo>();
            await foreach (var p in PhotoHandler.FindPhotosAsync(tempDirectory))
                photoList.Add(p);

            var photo = photoList.FirstOrDefault();
            var actualPath = photo.AbsoluteFilePath;

            // Assert the two paths are equal
            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public async Task FindPhotosAsync_MultiLevelDirectory_MultipleFiles_JpegFormatAsync()
        {
            // Create temporary directory
            var tempDirectory = GetTemporaryDirectory();
            
            // Find expected paths when creating files
            var expectedPaths = new HashSet<string>();
            for (var i = 0; i < 10; i++)
            {
                // Creates list of directories used for creating path with max depth
                var directoryList = new List<string> { tempDirectory };
                var rand = new Random().Next(0, MAXDEPTH);
                for (var k = 0; k < rand; k++)
                {
                    directoryList.Add(Path.GetRandomFileName());
                }

                // Combine the directory list and create path
                var tempPath = Path.Combine(directoryList.ToArray());
                Directory.CreateDirectory(tempPath);

                // Create file inside the generated directory
                var tempFileName = CreateTmpFile(tempPath, JpegFormat);

                // Adds path to expectedPaths
                var expectedPath = Path.Combine(tempPath, tempFileName);
                expectedPaths.Add(expectedPath);
            }

            // Find actual paths
            var photoList = new List<Photo>();
            await foreach (var p in PhotoHandler.FindPhotosAsync(tempDirectory))
                photoList.Add(p);

            var actualPaths = new HashSet<string>();
            foreach (var photo in photoList)
            {
                actualPaths.Add(photo.AbsoluteFilePath);
            }

            // Assert the two sets are equal
            Assert.Subset(expectedPaths, actualPaths);
            Assert.Superset(expectedPaths, actualPaths);
        }

        private string GetTemporaryDirectory()
        {
            // Generates temp directory path
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Creates temp directory
            Directory.CreateDirectory(tempDirectory);

            // Returns temp directory path
            return tempDirectory;
        }

        private string CreateTmpFile(string path, byte[] value = null)
        {
            if (value is null)
                value = new byte[2] { 0xff, 0xff };

            // Generates temp filename and path to file
            var tmpFileName = Guid.NewGuid().ToString();
            var tmpPath = Path.Combine(path, tmpFileName);

            // Creates the temp file
            File.WriteAllBytes(tmpPath, value);

            // Returns temp filename
            return tmpFileName;
        }
    }
}