using PhotoOrganizerLib.Models;
using System;
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

            Assert.Empty(photo.ImageMetadata);
            Assert.Equal(photoName, photo.Name);
            Assert.Equal(photoDir, photo.DirectoryPath);
            Assert.Equal(photoPath, photo.AbsoluteFilePath);
        }

        [Fact]
        public void AddMetadata_NotNullKey()
        {
            var key = "input";
            var value = 42;

            var photo = new Photo("");
            Assert.True(photo.TryAddMetadata(key, value));

            Assert.True(photo.ImageMetadata.ContainsKey(key));
            Assert.Equal(value, photo.ImageMetadata[key]);
        }

        [Fact]
        public void AddMetadata_DuplicateKey()
        {
            var key = "input";
            var value0 = 42;
            var value1 = value0 + 10;

            var photo = new Photo("");
            Assert.True(photo.TryAddMetadata(key, value0));
            Assert.False(photo.TryAddMetadata(key, value1));

            Assert.True(photo.ImageMetadata.ContainsKey(key));
            Assert.Equal(value0, photo.ImageMetadata[key]);
        }

        [Fact]
        public void AddMetadata_NullKey()
        {
            var photo = new Photo("");
            Assert.Throws<ArgumentNullException>(() => photo.TryAddMetadata(null, 42));
        }

        [Fact]
        public void TryPrintSpecificExifData_KeyExists()
        {
            var key = "input";
            var value = 42;

            var photo = new Photo("");
            photo.TryAddMetadata(key, value);

            Assert.True(photo.TryPrintSpecificExifData(key));
        }

        [Fact]
        public void TryPrintSpecificExifData_KeyDoesNotExist()
        {
            var key = "input";
            var photo = new Photo("");

            Assert.False(photo.TryPrintSpecificExifData(key));
        }

        [Fact]
        public void TryPrintSpecificExifData_NullKey()
        {
            var photo = new Photo("");

            Assert.False(photo.TryPrintSpecificExifData(null));
        }

        [Fact]
        public void PrintArrayExifData_EmptyDictionary()
        {
            var photo = new Photo("");

            photo.PrintArrayExifData();
        }

        [Fact]
        public void PrintArrayExifData_OneEntry()
        {
            var key = "input";
            var value = 42;

            var photo = new Photo("");
            photo.TryAddMetadata(key, value);

            photo.PrintArrayExifData();
        }
    }
}