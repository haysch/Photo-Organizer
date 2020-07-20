using PhotoOrganizerLib.Models;
using System.IO;
using Xunit;

namespace PhotoOrganizerLib.Tests.Models
{
    public class PhotoTest
    {
        [Fact]
        public void Constructor_PathAndDictionary()
        {
            var photoName = Path.GetRandomFileName();
            var photoDir = Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar);
            var photoPath = Path.Combine(photoDir, photoName);

            var photo = new Photo(photoPath);

            Assert.Equal(photoName, photo.Name);
            Assert.Equal(photoDir, photo.DirectoryPath);
            Assert.Equal(photoPath, photo.FilePath);
        }

        [Fact]
        public void Constructor_EmptyPath()
        {
            var photo = new Photo(string.Empty);

            Assert.Empty(photo.Name);
            Assert.Null(photo.DirectoryPath);
        }

        [Fact]
        public void Constructor_NullPath()
        {
            var photo = new Photo(null);

            Assert.Null(photo.Name);
            Assert.Null(photo.DirectoryPath);
        }

        [Fact]
        public void SetFilePath()
        {
            var photo = new Photo(null);

            Assert.Null(photo.Name);
            Assert.Null(photo.DirectoryPath);

            var filename = Path.GetRandomFileName();
            var tempDirectory = Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar); // remove final separator if any
            photo.FilePath = Path.Combine(tempDirectory, filename);

            Assert.Equal(filename, photo.Name);
            Assert.Equal(tempDirectory, photo.DirectoryPath);
        }
    }
}