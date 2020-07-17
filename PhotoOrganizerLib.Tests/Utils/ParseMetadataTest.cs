using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Utils;
using Xunit;

namespace PhotoOrganizerLib.Tests.Utils
{
    public class ParseMetadataTest
    {
        [Fact]
        public void ParseDirectories()
        {
            // Align the data formatting for testing
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            
            // SubIFD
            float fNumber = 12;
            short iso = 1200;
            var apexValue = 1;
            var dateTimeOriginal = DateTime.Today;
            float focalLength = 30;

            // JPEG
            var height = 32;
            var width = 32;

            var directories = new List<Directory> 
            {
                CreateSubIfdDirectory(fNumber, iso, apexValue, dateTimeOriginal, focalLength),
                CreateJpegDirectory(height, width)
            };
            var photo = new Photo("");

            ParseMetadata.Parse(photo, directories);

            // Assert JPEG metadata
            Assert.Equal(height, photo.ImageMetadata["Height"]);
            Assert.Equal(width, photo.ImageMetadata["Width"]);

            // Assert SubIFD metadata
            Assert.Equal(fNumber, photo.ImageMetadata["F-Number"]);
            Assert.Equal(iso, photo.ImageMetadata["ISO"]);
            Assert.Equal("0.5 sec", photo.ImageMetadata["ShutterSpeed"]); // round((1 / exp(1 * log(2))) * 10) / 10 = 0.5
            Assert.Equal(dateTimeOriginal, photo.ImageMetadata["DateTimeOriginal"]);
            Assert.Equal(focalLength, photo.ImageMetadata["FocalLength"]);
        }

        private ExifSubIfdDirectory CreateSubIfdDirectory(float fNumber, short iso, float apexValue, DateTime dateTimeOriginal, float focalLength)
        {
            

            var subIfdDirectory = new ExifSubIfdDirectory();

            subIfdDirectory.Set(ExifSubIfdDirectory.TagFNumber, fNumber);
            subIfdDirectory.Set(ExifSubIfdDirectory.TagIsoEquivalent, iso);
            subIfdDirectory.Set(ExifSubIfdDirectory.TagShutterSpeed, apexValue);
            subIfdDirectory.Set(ExifSubIfdDirectory.TagDateTimeOriginal, dateTimeOriginal);
            subIfdDirectory.Set(ExifSubIfdDirectory.TagFocalLength, focalLength);

            return subIfdDirectory;
        }

        private JpegDirectory CreateJpegDirectory(int height, int width)
        {
            

            var jpegDirectory = new JpegDirectory();

            jpegDirectory.Set(JpegDirectory.TagImageHeight, height);
            jpegDirectory.Set(JpegDirectory.TagImageWidth, width);

            return jpegDirectory;
        }
    }
}