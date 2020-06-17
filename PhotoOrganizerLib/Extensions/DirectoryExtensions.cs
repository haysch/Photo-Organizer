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
        public static void ParseGps(this GpsDirectory directory, Photo photo)
        {
            var gpsLatRef = directory.GetString(GpsDirectory.TagLatitudeRef);
            var gpsLat = directory.GetRationalArray(GpsDirectory.TagLatitude);

            var gpsLonRef = directory.GetString(GpsDirectory.TagLongitudeRef);
            var gpsLon = directory.GetRationalArray(GpsDirectory.TagLongitude);

            var latitude = MetadataConverter.DegreesMinutesSecondsToDecimal(gpsLat, gpsLatRef);
            var longitude = MetadataConverter.DegreesMinutesSecondsToDecimal(gpsLon, gpsLonRef);

            photo.AddMetadata("Latitude", latitude);
            photo.AddMetadata("Longitude", longitude);
            
            if (directory.TryGetByte(GpsDirectory.TagAltitudeRef, out var gpsAltBit) &&
                directory.TryGetInt16(GpsDirectory.TagAltitude, out var gpsAlt))
            {
                var gpsAltRef = gpsAltBit == 0 ? "Sea level" : "Below sea level";
                photo.AddMetadata("AltitudeRef", gpsAltRef);
                photo.AddMetadata("Altitude", gpsAlt);
            }
        }

        public static void ParseIFD0(this ExifIfd0Directory directory, Photo photo)
        {
            var make = directory.GetString(ExifIfd0Directory.TagMake);
            var model = directory.GetString(ExifIfd0Directory.TagModel);

            if (make != null)
                photo.AddMetadata("Make", make);

            if (model != null)
                photo.AddMetadata("Model", model);

            if (directory.TryGetDateTime(ExifIfd0Directory.TagDateTime, out var datetime))
                photo.AddMetadata("DateTime", datetime);
        }

        public static void ParseSubIfd(this ExifSubIfdDirectory directory, Photo photo)
        {
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

        public static void Parse(this JpegDirectory directory, Photo photo)
        {
            if (directory.TryGetInt32(JpegDirectory.TagImageHeight, out var height))
                photo.AddMetadata("Height", height);

            if (directory.TryGetInt32(JpegDirectory.TagImageWidth, out var width))
                photo.AddMetadata("Width", width);
        }

        public static void Parse(this PngDirectory directory, Photo photo)
        {
            if (directory.TryGetInt32(PngDirectory.TagImageHeight, out var height))
                photo.AddMetadata("Height", height);

            if (directory.TryGetInt32(PngDirectory.TagImageWidth, out var width))
                photo.AddMetadata("Width", width);
        }
    }
}