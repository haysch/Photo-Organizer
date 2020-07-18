using System;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Png;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Utils;

namespace PhotoOrganizerLib.Extensions
{
    public static class DirectoryExtensions
    {
        /// <summary>Parses <see cref="MetadataExtractor.Formats.Exif.GpsDirectory" /> metadata and saves it to the <see cref="PhotoOrganizerLib.Models.Photo" />.</summary>
        /// <param name="directory">Directory containing the GPS metadata.</param>
        /// <param name="photo">Photo object used for storing metadata.</param>
        public static void Parse(this GpsDirectory directory, Photo photo)
        {
            if (directory is null || photo is null)
            {
                return;
            }

            var gpsLatRef = directory.GetString(GpsDirectory.TagLatitudeRef);
            var gpsLat = directory.GetRationalArray(GpsDirectory.TagLatitude);

            var gpsLonRef = directory.GetString(GpsDirectory.TagLongitudeRef);
            var gpsLon = directory.GetRationalArray(GpsDirectory.TagLongitude);

            var latitude = MetadataConverter.DegreesMinutesSecondsToDecimalDegrees(gpsLat, gpsLatRef);
            var longitude = MetadataConverter.DegreesMinutesSecondsToDecimalDegrees(gpsLon, gpsLonRef);

            // if we do not have both latitude and longitude, do not save coordinates
            if (!(latitude is null) && !(longitude is null))
            {
                photo.TryAddMetadata("Latitude", latitude);
                photo.TryAddMetadata("Longitude", longitude);
            }
            
            // if we cannot get both the altitude and its reference, do not save anything
            if (directory.TryGetByte(GpsDirectory.TagAltitudeRef, out var gpsAltBit) &&
                directory.TryGetInt16(GpsDirectory.TagAltitude, out var gpsAlt))
            {
                var gpsAltRef = gpsAltBit == 0 ? "Sea level" : "Below sea level";
                photo.TryAddMetadata("AltitudeRef", gpsAltRef);
                photo.TryAddMetadata("Altitude", gpsAlt);
            }
        }

        /// <summary>Parses <see cref="MetadataExtractor.Formats.Exif.ExifIfd0Directory" /> metadata and saves it to the <see cref="PhotoOrganizerLib.Models.Photo" />.</summary>
        /// <param name="directory">Directory containing the make, model and datetime metadata.</param>
        /// <param name="photo">Photo object used for storing metadata.</param>
        public static void Parse(this ExifIfd0Directory directory, Photo photo)
        {
            if (directory is null || photo is null)
            {
                return;
            }

            var make = directory.GetString(ExifIfd0Directory.TagMake);
            var model = directory.GetString(ExifIfd0Directory.TagModel);

            if (!(make is null))
                photo.TryAddMetadata("Make", make);

            if (!(model is null))
                photo.TryAddMetadata("Model", model);

            if (directory.TryGetDateTime(ExifIfd0Directory.TagDateTime, out var datetime))
                photo.TryAddMetadata("DateTime", datetime);
        }

        /// <summary>Parses <see cref="MetadataExtractor.Formats.Exif.ExifSubIfdDirectory" /> metadata and saves it to the <see cref="PhotoOrganizerLib.Models.Photo" />.</summary>
        /// <param name="directory">Directory containing the F-number, ISO, shutter speed, datetime original and focal length metadata.</param>
        /// <param name="photo">Photo object used for storing metadata.</param>
        public static void Parse(this ExifSubIfdDirectory directory, Photo photo)
        {
            if (directory is null || photo is null)
            {
                return;
            }

            if (directory.TryGetSingle(ExifDirectoryBase.TagFNumber, out var fNum))
                photo.TryAddMetadata("F-Number", fNum);

            if (directory.TryGetInt16(ExifDirectoryBase.TagIsoEquivalent, out var iso))
                photo.TryAddMetadata("ISO", iso);
            
            if (directory.TryGetSingle(ExifDirectoryBase.TagShutterSpeed, out var apexValue))
                photo.TryAddMetadata("ShutterSpeed", MetadataConverter.ComputeShutterSpeed(apexValue));

            if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTimeOriginal))
                photo.TryAddMetadata("DateTimeOriginal", dateTimeOriginal);

            if (directory.TryGetSingle(ExifDirectoryBase.TagFocalLength, out var focalLength))
                photo.TryAddMetadata("FocalLength", focalLength);
        }

        /// <summary>Parses <see cref="MetadataExtractor.Formats.Jpeg.JpegDirectory" /> metadata and saves it to the <see cref="PhotoOrganizerLib.Models.Photo" />.</summary>
        /// <param name="directory">Directory containing the JPEG image height and width.</param>
        /// <param name="photo">Photo object used for storing metadata.</param>
        public static void Parse(this JpegDirectory directory, Photo photo)
        {
            if (directory is null || photo is null)
            {
                return;
            }
            
            if (directory.TryGetInt32(JpegDirectory.TagImageHeight, out var height))
                photo.TryAddMetadata("Height", height);

            if (directory.TryGetInt32(JpegDirectory.TagImageWidth, out var width))
                photo.TryAddMetadata("Width", width);
        }

        /// <summary>Parses <see cref="MetadataExtractor.Formats.Png.PngDirectory" /> metadata and saves it to the <see cref="PhotoOrganizerLib.Models.Photo" />.</summary>
        /// <param name="directory">Directory containing the PNG image height and width.</param>
        /// <param name="photo">Photo object used for storing metadata.</param>
        public static void Parse(this PngDirectory directory, Photo photo)
        {
            if (directory is null || photo is null)
            {
                return;
            }

            if (directory.TryGetInt32(PngDirectory.TagImageHeight, out var height))
                photo.TryAddMetadata("Height", height);

            if (directory.TryGetInt32(PngDirectory.TagImageWidth, out var width))
                photo.TryAddMetadata("Width", width);
        }
    }
}