using Xunit;

using PhotoOrganizerLib.Extensions;

using MetadataExtractor.Formats.Exif;
using PhotoOrganizerLib.Models;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Png;

namespace PhotoOrganizerTest
{
    public class ExtensionsTests
    {
        [Fact]
        public void ParseGpsDirectory_NullDirectory_NoMetadata()
        {
            GpsDirectory gpsDirectory = null;
            var photo = new Photo("TestPhotoName","");

            gpsDirectory.ParseGps(photo);

            Assert.Empty(photo.ImageMetadata);
        }
        
        [Fact]
        public void ParseIfd0Directory_NullDirectory_NoMetadata()
        {
            ExifIfd0Directory ifd0Directory = null;
            var photo = new Photo("TestPhotoName","");

            ifd0Directory.ParseIFD0(photo);

            Assert.Empty(photo.ImageMetadata);
        }

        [Fact]
        public void ParseSubIfdDirectory_NullDirectory_NoMetadata()
        {
            ExifSubIfdDirectory subIfdDirectory = null;
            var photo = new Photo("TestPhotoName","");

            subIfdDirectory.ParseSubIfd(photo);

            Assert.Empty(photo.ImageMetadata);
        }

        [Fact]
        public void ParseJpegDirectory_NullDirectory_NoMetadata()
        {
            JpegDirectory jpegDirectory = null;
            var photo = new Photo("TestPhotoName","");

            jpegDirectory.Parse(photo);

            Assert.Empty(photo.ImageMetadata);
        }

        [Fact]
        public void ParsePngDirectory_NullDirectory_NoMetadata()
        {
            PngDirectory pngDirectory = null;
            var photo = new Photo("TestPhotoName","");

            pngDirectory.Parse(photo);

            Assert.Empty(photo.ImageMetadata);
        }

        [Fact]
        public void ParseGpsDirectory_ValidDirectory_Metadata()
        {
        
        }
    }
}