using System;
using System.Collections.Generic;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Png;
using PhotoOrganizer.Models;

namespace PhotoOrganizer.Util
{
    /// <summary>Class for parsing <see cref="MetadataExtractor.Directory" />.</summary>
    public static class ParseMetadata
    {
        /// <summary>Parses the dictionary and saves the metadata to the <see cref="PhotoOrganizer.Models.Picture" /> object.</summary>
        /// <param name="picture">Picture object used for extracting metadata and saving the values.</param>
        /// <param name="directories">Enumerable of <see cref="MetadataExtractor.Directory" />, containing the different image <see cref="MetadataExtractor.Tag" />s.</param>
        public static void Parse(Picture picture, IEnumerable<Directory> directories)
        {
            foreach (var directory in directories)
            {
                switch (directory.Name)
                {
                    case "GPS":
                        ParseGps(directory, picture);
                        continue;

                    case "Exif IFD0":
                        ParseIFD0(directory, picture);
                        continue;

                    case "Exif SubIFD":
                        ParseSubIfd(directory, picture);
                        continue;

                    case "JPEG":
                        ParseJpeg(directory, picture);
                        continue;

                    case "PNG":
                        ParsePng(directory, picture);
                        continue;

                    default:
                        continue;
                }
            }
        }

        private static void ParseGps(Directory directory, Picture picture)
        {
            var gpsLatRef = directory.GetString(GpsDirectory.TagLatitudeRef);
            var gpsLat = directory.GetRationalArray(GpsDirectory.TagLatitude);

            var gpsLonRef = directory.GetString(GpsDirectory.TagLongitudeRef);
            var gpsLon = directory.GetRationalArray(GpsDirectory.TagLongitude);

            var latitude = DegreesMinutesSecondsToDecimal(gpsLat, gpsLatRef);
            var longitude = DegreesMinutesSecondsToDecimal(gpsLon, gpsLonRef);

            picture.AddMetadata("Latitude", latitude);
            picture.AddMetadata("Longitude", longitude);
            
            if (directory.TryGetByte(GpsDirectory.TagAltitudeRef, out var gpsAltBit) &&
                directory.TryGetInt16(GpsDirectory.TagAltitude, out var gpsAlt))
            {
                var gpsAltRef = gpsAltBit == 0 ? "Sea level" : "Below sea level";
                picture.AddMetadata("AltitudeRef", gpsAltRef);
                picture.AddMetadata("Altitude", gpsAlt);
            }
        }

        private static void ParseIFD0(Directory directory, Picture picture)
        {
            var make = directory.GetString(ExifIfd0Directory.TagMake);
            var model = directory.GetString(ExifIfd0Directory.TagModel);

            if (make != null)
                picture.AddMetadata("Make", make);

            if (model != null)
                picture.AddMetadata("Model", model);

            if (directory.TryGetDateTime(ExifIfd0Directory.TagDateTime, out var datetime))
                picture.AddMetadata("DateTime", datetime);
        }

        private static void ParseSubIfd(Directory directory, Picture picture)
        {
            if (directory.TryGetSingle(ExifDirectoryBase.TagFNumber, out var fNum))
                picture.AddMetadata("F-Number", fNum);

            if (directory.TryGetInt16(ExifDirectoryBase.TagIsoEquivalent, out var iso))
                picture.AddMetadata("ISO", iso);
            
            if (directory.TryGetSingle(ExifDirectoryBase.TagShutterSpeed, out var apexValue))
                picture.AddMetadata("ShutterSpeed", ComputeShutterSpeed(apexValue));

            if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTimeOriginal))
                picture.AddMetadata("DateTimeOriginal", dateTimeOriginal);

            if (directory.TryGetSingle(ExifDirectoryBase.TagFocalLength, out var focalLength))
                picture.AddMetadata("FocalLength", focalLength);
        }

        private static void ParseJpeg(Directory directory, Picture picture)
        {
            if (directory.TryGetInt32(JpegDirectory.TagImageHeight, out var height))
                picture.AddMetadata("Height", height);

            if (directory.TryGetInt32(JpegDirectory.TagImageWidth, out var width))
                picture.AddMetadata("Width", width);
        }

        private static void ParsePng(Directory directory, Picture picture)
        {
            if (directory.TryGetInt32(PngDirectory.TagImageHeight, out var height))
                picture.AddMetadata("Height", height);

            if (directory.TryGetInt32(PngDirectory.TagImageWidth, out var width))
                picture.AddMetadata("Width", width);
        }

        /// <summary>Private method for converting Degree/Minutes/Seconds to decimal degrees.</summary>
        /// <returns>Double of the coordinate.</returns>
        /// <remarks>If input array is not of size 3, return 0.0.</remarks>
        /// <param name="degMinSec">Rational array containing the Degree/Minutes/Seconds.</param>
        /// <param name="gpsRef">GPS reference specifying direction, e.g. "N" or "E".</param>
        public static double DegreesMinutesSecondsToDecimal(Rational[] degMinSec, string gpsRef)
        {
            if (degMinSec?.Length != 3 || gpsRef is null) 
                return 0;

            var hours = Math.Abs(degMinSec[0].ToDouble());
            double minutes = degMinSec[1].ToDouble();
            double seconds = degMinSec[2].ToDouble();

            double value = hours + (minutes / 60.0d) + (seconds / 3600.0d);

            // If Ref is not N or E, negate the value.
            if (gpsRef == "S" || gpsRef == "W")
                value *= -1;
            
            return value;
        }

        private static string ComputeShutterSpeed(float apexValue)
        {
            if (apexValue <= 1)
            {
                var apexPower = (float)(1 / Math.Exp(apexValue * Math.Log(2)));
                var apexPower10 = (long)Math.Round(apexPower * 10.0);
                var fApexPower = apexPower10 / 10.0f;
                return fApexPower + " sec";
            }
            else
            {
                var apexPower = (int)Math.Exp(apexValue * Math.Log(2));
                return "1/" + apexPower + " sec";
            }
        }
    }
}