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
        /// <summary>Default parse method for handling unsupported directories.</summary>
        /// <param name="directory">Directory containing the some metadata.</param>
        /// <param name="photo">Photo object used for storing metadata.</param>
        /// <remarks>This method is meant as a handler where the directory parsing has not been implemented yet.</remarks>
        public static void Parse(this Directory directory, Photo photo)
        {
            Console.WriteLine($"Directory type { directory.GetType() } is not supported for parsing.");
        }

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
                photo.AddMetadata("Latitude", latitude);
                photo.AddMetadata("Longitude", longitude);
            }
            
            // if we cannot get both the altitude and its reference, do not save anything
            if (directory.TryGetByte(GpsDirectory.TagAltitudeRef, out var gpsAltBit) &&
                directory.TryGetInt16(GpsDirectory.TagAltitude, out var gpsAlt))
            {
                var gpsAltRef = gpsAltBit == 0 ? "Sea level" : "Below sea level";
                photo.AddMetadata("AltitudeRef", gpsAltRef);
                photo.AddMetadata("Altitude", gpsAlt);
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
                photo.AddMetadata("Make", make);

            if (!(model is null))
                photo.AddMetadata("Model", model);

            if (directory.TryGetDateTime(ExifIfd0Directory.TagDateTime, out var datetime))
                photo.AddMetadata("DateTime", datetime);
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
                photo.AddMetadata("F-Number", fNum);

            if (directory.TryGetInt16(ExifDirectoryBase.TagIsoEquivalent, out var iso))
                photo.AddMetadata("ISO", iso);
            
            if (directory.TryGetSingle(ExifDirectoryBase.TagShutterSpeed, out var apexValue))
                photo.AddMetadata("ShutterSpeed", MetadataConverter.ComputeShutterSpeed(apexValue));

            if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTimeOriginal))
                photo.AddMetadata("DateTimeOriginal", dateTimeOriginal);

            if (directory.TryGetSingle(ExifDirectoryBase.TagFocalLength, out var focalLength))
                photo.AddMetadata("FocalLength", focalLength);
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
                photo.AddMetadata("Height", height);

            if (directory.TryGetInt32(JpegDirectory.TagImageWidth, out var width))
                photo.AddMetadata("Width", width);
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
                photo.AddMetadata("Height", height);

            if (directory.TryGetInt32(PngDirectory.TagImageWidth, out var width))
                photo.AddMetadata("Width", width);
        }
    }
}