using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Utils;
using Xunit;
using System.Drawing.Imaging;

namespace PhotoOrganizerLib.Tests.Utils.Tests
{
    public class PhotoHandlerTest
    {
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
            var tempDirectory = PathHelper.GetTemporaryDirectory();
            PathHelper.CreateTmpFile(tempDirectory); // create tmp file with unsupported filetype

            var photoList = new List<Photo>();
            await foreach (var photo in PhotoHandler.FindPhotosAsync(tempDirectory))
                photoList.Add(photo);

            Assert.Empty(photoList);
        }

        [Fact]
        public async Task FindPhotosAsync_FlatDirectory_SingleFile_JpegFormatAsync()
        {
            var tempDirectory = PathHelper.GetTemporaryDirectory();
            var tempPath = PathHelper.CreateImageFile(tempDirectory, ImageFormat.Jpeg);
            var expectedPath = Path.Combine(tempDirectory, tempPath);

            var photoList = new List<Photo>();
            await foreach (var p in PhotoHandler.FindPhotosAsync(tempDirectory))
                photoList.Add(p);

            var photo = photoList.FirstOrDefault();
            var actualPath = photo.FilePath;

            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public async Task FindPhotosAsync_FlatDirectory_MultipleFiles_JpegFormatAsync()
        {
            var tempDirectory = PathHelper.GetTemporaryDirectory();

            // Creates the expected paths set from the file creation
            var expectedPaths = new HashSet<string>();
            for (var i = 0; i < 10; i++)
            {
                var path = Path.Combine(tempDirectory, PathHelper.CreateImageFile(tempDirectory, ImageFormat.Jpeg));
                expectedPaths.Add(path);
            }

            // Creates set containing the actual paths
            var photoList = new List<Photo>();
            await foreach (var p in PhotoHandler.FindPhotosAsync(tempDirectory))
                photoList.Add(p);

            var actualPaths = new HashSet<string>();
            foreach (var photo in photoList)
            {
                actualPaths.Add(photo.FilePath);
            }

            // Assert the two sets are equal
            Assert.Subset(expectedPaths, actualPaths);
            Assert.Superset(expectedPaths, actualPaths);
        }

        [Fact]
        public async Task FindPhotosAsync_TwoLevelDirectory_SingleFile_JpegFormatAsync()
        {
            // Create deeper directory structure
            var tempDirectory = PathHelper.GetTemporaryDirectory();
            var newDirectory = Path.Combine(tempDirectory, Path.GetRandomFileName());
            Directory.CreateDirectory(newDirectory);

            // Find expected path from the file creation
            var tempFileName = PathHelper.CreateImageFile(newDirectory, ImageFormat.Jpeg);
            var expectedPath = Path.Combine(newDirectory, tempFileName);

            // Find actual path
            var photoList = new List<Photo>();
            await foreach (var p in PhotoHandler.FindPhotosAsync(tempDirectory))
                photoList.Add(p);

            var photo = photoList.FirstOrDefault();
            var actualPath = photo.FilePath;

            // Assert the two paths are equal
            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public async Task FindPhotosAsync_MultiLevelDirectory_MultipleFiles_JpegFormatAsync()
        {
            // Create temporary directory
            var tempDirectory = PathHelper.GetTemporaryDirectory();
            
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
                var tempFileName = PathHelper.CreateImageFile(tempPath, ImageFormat.Jpeg);

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
                actualPaths.Add(photo.FilePath);
            }

            // Assert the two sets are equal
            Assert.Subset(expectedPaths, actualPaths);
            Assert.Superset(expectedPaths, actualPaths);
        }
    }
}