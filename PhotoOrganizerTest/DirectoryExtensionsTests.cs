using System;
using Xunit;

using PhotoOrganizerLib.Extensions;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Utils;

using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Png;

using PhotoOrganizerTest.Models;

namespace PhotoOrganizerTest
{
    public class ExtensionsTests
    {
        [Fact]
        public void ParseGpsDirectory_NullDirectory_NoMetadata()
        {
            GpsDirectory gpsDirectory = null;
            var photo = new Photo("");

            gpsDirectory.Parse(photo);

            Assert.Empty(photo.ImageMetadata);
        }
        
        [Fact]
        public void ParseIfd0Directory_NullDirectory_NoMetadata()
        {
            ExifIfd0Directory ifd0Directory = null;
            var photo = new Photo("");

            ifd0Directory.Parse(photo);

            Assert.Empty(photo.ImageMetadata);
        }

        [Fact]
        public void ParseSubIfdDirectory_NullDirectory_NoMetadata()
        {
            ExifSubIfdDirectory subIfdDirectory = null;
            var photo = new Photo("");

            subIfdDirectory.Parse(photo);

            Assert.Empty(photo.ImageMetadata);
        }

        [Fact]
        public void ParseJpegDirectory_NullDirectory_NoMetadata()
        {
            JpegDirectory jpegDirectory = null;
            var photo = new Photo("");

            jpegDirectory.Parse(photo);

            Assert.Empty(photo.ImageMetadata);
        }

        [Fact]
        public void ParsePngDirectory_NullDirectory_NoMetadata()
        {
            PngDirectory pngDirectory = null;
            var photo = new Photo("");

            pngDirectory.Parse(photo);

            Assert.Empty(photo.ImageMetadata);
        }

        [Fact]
        public void ParseGpsDirectory_ValidDirectory_LatLongMetadata()
        {
            var gpsDirectory = new GpsDirectory();
            var photo = new Photo("");

            var gpsReference = "N";
            var gpsRationalArr = new Rational[3]
            { 
                new Rational(1, 1),
                new Rational(60, 1),
                new Rational(3600, 1)
            };

            gpsDirectory.Set(GpsDirectory.TagLatitudeRef, gpsReference);
            gpsDirectory.Set(GpsDirectory.TagLatitude, gpsRationalArr);

            gpsDirectory.Set(GpsDirectory.TagLongitudeRef, gpsReference);
            gpsDirectory.Set(GpsDirectory.TagLongitude, gpsRationalArr);

            gpsDirectory.Parse(photo);

            var actualLatitude = photo.ImageMetadata["Latitude"];
            var actualLongitude = photo.ImageMetadata["Longitude"];
            
            var expectedLatitude = MetadataConverter.DegreesMinutesSecondsToDecimalDegrees(gpsRationalArr, gpsReference);
            var expectedLongitude = MetadataConverter.DegreesMinutesSecondsToDecimalDegrees(gpsRationalArr, gpsReference);

            Assert.Equal(expectedLatitude, actualLatitude);
            Assert.Equal(expectedLongitude, actualLongitude);
        }

        [Fact]
        public void ParseGpsDirectory_ValidDirectory_AltMetadata()
        {
            var gpsDirectory = new GpsDirectory();
            var photo = new Photo("");

            var gpsAltBit = 0;
            short gpsAlt = 50;

            gpsDirectory.Set(GpsDirectory.TagAltitudeRef, gpsAltBit);
            gpsDirectory.Set(GpsDirectory.TagAltitude, gpsAlt);

            gpsDirectory.Parse(photo);

            var actualAltitudeRef = photo.ImageMetadata["AltitudeRef"];
            var actualAltitude = photo.ImageMetadata["Altitude"];

            var expectedAltitudeRef = "Sea level";

            Assert.Equal(expectedAltitudeRef, actualAltitudeRef);
            Assert.Equal(gpsAlt, actualAltitude);
        }

        [Fact]
        public void ParseIfd0Directory_ValidDirectory_AllMetadata()
        {
            var ifd0Directory = new ExifIfd0Directory();
            var photo = new Photo("");

            var make = "TestMake";
            var model = "TestModel";
            var dateTime = DateTime.Today;

            ifd0Directory.Set(ExifIfd0Directory.TagMake, make);
            ifd0Directory.Set(ExifIfd0Directory.TagModel, model);
            ifd0Directory.Set(ExifIfd0Directory.TagDateTime, dateTime);

            ifd0Directory.Parse(photo);

            var actualMake = photo.ImageMetadata["Make"];
            var actualModel = photo.ImageMetadata["Model"];
            var actualDateTime = photo.ImageMetadata["DateTime"];

            Assert.NotEmpty(photo.ImageMetadata);
            Assert.Equal(make, actualMake);
            Assert.Equal(model, actualModel);
            Assert.Equal(dateTime, actualDateTime);
        }

        [Fact]
        public void ParseSubIfd_ValidDirectory_AllMetadata()
        {
            var subIfdDirectory = new ExifSubIfdDirectory();
            var photo = new Photo("");

            float fNumber = 12;
            short iso = 1200;
            var apexValue = 1;
            var dateTimeOriginal = DateTime.Today;
            float focalLength = 30;

            subIfdDirectory.Set(ExifSubIfdDirectory.TagFNumber, fNumber);
            subIfdDirectory.Set(ExifSubIfdDirectory.TagIsoEquivalent, iso);
            subIfdDirectory.Set(ExifSubIfdDirectory.TagShutterSpeed, apexValue);
            subIfdDirectory.Set(ExifSubIfdDirectory.TagDateTimeOriginal, dateTimeOriginal);
            subIfdDirectory.Set(ExifSubIfdDirectory.TagFocalLength, focalLength);

            subIfdDirectory.Parse(photo);

            var expectedShutterSpeed = "0.5 sec"; // round((1 / exp(1 * log(2))) * 10) / 10 = 0.5
            
            var actualFNumber = photo.ImageMetadata["F-Number"];
            var actualIso = photo.ImageMetadata["ISO"];
            var actualShutterSpeed = photo.ImageMetadata["ShutterSpeed"];
            var actualDateTimeOriginal = photo.ImageMetadata["DateTimeOriginal"];
            var actualFocalLength = photo.ImageMetadata["FocalLength"];

            Assert.Equal(fNumber, actualFNumber);
            Assert.Equal(iso, actualIso);
            Assert.Equal(expectedShutterSpeed, actualShutterSpeed);
            Assert.Equal(dateTimeOriginal, actualDateTimeOriginal);
            Assert.Equal(focalLength, actualFocalLength);
        }

        [Fact]
        public void ParseJpegDirectory_ValidDirectory_AllMetadata()
        {
            var jpegDirectory = new JpegDirectory();
            var photo = new Photo("");

            var height = 32;
            var width = 32;

            jpegDirectory.Set(JpegDirectory.TagImageHeight, height);
            jpegDirectory.Set(JpegDirectory.TagImageWidth, width);

            jpegDirectory.Parse(photo);

            var actualHeight = photo.ImageMetadata["Height"];
            var actualWidth = photo.ImageMetadata["Width"];

            Assert.Equal(height, actualHeight);
            Assert.Equal(width, actualWidth);
        }

        [Fact]
        public void ParsePngDirectory_ValidDirectory_AllMetadata()
        {
            var pngDirectory = new PngDirectory(PngChunkType.bKGD);
            var photo = new Photo("");

            var height = 32;
            var width = 32;

            pngDirectory.Set(PngDirectory.TagImageHeight, height);
            pngDirectory.Set(PngDirectory.TagImageWidth, width);

            pngDirectory.Parse(photo);

            var actualHeight = photo.ImageMetadata["Height"];
            var actualWidth = photo.ImageMetadata["Width"];

            Assert.Equal(height, actualHeight);
            Assert.Equal(width, actualWidth);
        }

        [Fact]
        public void ParseUnsupportedDirectory()
        {
            var testDirectory = new TestDirectory();
            var photo = new Photo( "");

            testDirectory.Parse(photo);

            Assert.Empty(photo.ImageMetadata);
        }
    }
}