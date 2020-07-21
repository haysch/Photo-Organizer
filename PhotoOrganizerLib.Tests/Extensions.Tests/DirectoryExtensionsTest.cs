using System;
using Xunit;

using PhotoOrganizerLib.Extensions;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Utils;

using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Png;

using PhotoOrganizerLib.Tests.Models;
using System.Threading;
using System.Globalization;

namespace PhotoOrganizerLib.Tests.Extensions.Tests
{
    public class ExtensionsTest
    {
        [Fact]
        public void ParseGpsDirectory_NullDirectory_NoMetadata()
        {
            GpsDirectory gpsDirectory = null;
            var photo = new Photo("");

            gpsDirectory.Parse(photo);

            Assert.Null(photo.Latitude);
            Assert.Null(photo.Longitude);
            Assert.Null(photo.AltitudeReference);
            Assert.Null(photo.Altitude);
        }
        
        [Fact]
        public void ParseIfd0Directory_NullDirectory_NoMetadata()
        {
            ExifIfd0Directory ifd0Directory = null;
            var photo = new Photo("");

            ifd0Directory.Parse(photo);

            Assert.Null(photo.Make);
            Assert.Null(photo.Model);
            Assert.Null(photo.DateTime);
        }

        [Fact]
        public void ParseSubIfdDirectory_NullDirectory_NoMetadata()
        {
            ExifSubIfdDirectory subIfdDirectory = null;
            var photo = new Photo("");

            subIfdDirectory.Parse(photo);

            Assert.Null(photo.FNumber);
            Assert.Null(photo.Iso);
            Assert.Null(photo.ShutterSpeed);
            Assert.Null(photo.DateTimeOriginal);
            Assert.Null(photo.FocalLength);
        }

        [Fact]
        public void ParseJpegDirectory_NullDirectory_NoMetadata()
        {
            JpegDirectory jpegDirectory = null;
            var photo = new Photo("");

            jpegDirectory.Parse(photo);

            Assert.Equal(0, photo.Height);
            Assert.Equal(0, photo.Width);
        }

        [Fact]
        public void ParsePngDirectory_NullDirectory_NoMetadata()
        {
            PngDirectory pngDirectory = null;
            var photo = new Photo("");

            pngDirectory.Parse(photo);

            Assert.Equal(0, photo.Height);
            Assert.Equal(0, photo.Width);
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

            var actualLatitude = photo.Latitude;
            var actualLongitude = photo.Longitude;
            
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

            var actualAltitudeRef = photo.AltitudeReference;
            var actualAltitude = photo.Altitude;

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

            var actualMake = photo.Make;
            var actualModel = photo.Model;
            var actualDateTime = photo.DateTime;

            Assert.Equal(make, actualMake);
            Assert.Equal(model, actualModel);
            Assert.Equal(dateTime, actualDateTime);
        }

        [Fact]
        public void ParseSubIfd_ValidDirectory_AllMetadata()
        {
            // Align the data formatting for testing
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

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
            
            var actualFNumber = photo.FNumber;
            var actualIso = photo.Iso;
            var actualShutterSpeed = photo.ShutterSpeed;
            var actualDateTimeOriginal = photo.DateTimeOriginal;
            var actualFocalLength = photo.FocalLength;

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

            var actualHeight = photo.Height;
            var actualWidth = photo.Width;

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

            var actualHeight = photo.Height;
            var actualWidth = photo.Width;

            Assert.Equal(height, actualHeight);
            Assert.Equal(width, actualWidth);
        }
    }
}