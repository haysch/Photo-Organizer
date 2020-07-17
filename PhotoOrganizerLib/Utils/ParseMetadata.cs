using System.Collections.Generic;
using System.Linq;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Png;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Extensions;

namespace PhotoOrganizerLib.Utils
{
    /// <summary>Class for parsing <see cref="MetadataExtractor.Directory" /> to <see cref="PhotoOrganizerLib.Models.Photo" /> metadata dictionary.</summary>
    public static class ParseMetadata
    {
        /// <summary>Parses the list of <see cref="MetadataExtractor.Directory" /> and saves the metadata to the <see cref="PhotoOrganizerLib.Models.Photo" /> object.</summary>
        /// <param name="photo">Picture object used for extracting metadata and saving the values.</param>
        /// <param name="directories">Enumerable of <see cref="MetadataExtractor.Directory" />, containing the different image <see cref="MetadataExtractor.Tag" />s.</param>
        public static void Parse(Photo photo, IEnumerable<Directory> directories)
        {
            directories.OfType<ExifSubIfdDirectory>()
                .FirstOrDefault()?
                .Parse(photo);

            directories.OfType<GpsDirectory>()
                .FirstOrDefault()?
                .Parse(photo);

            directories.OfType<ExifIfd0Directory>()
                .FirstOrDefault()?
                .Parse(photo);

            directories.OfType<JpegDirectory>()
                .FirstOrDefault()?
                .Parse(photo);

            directories.OfType<PngDirectory>()
                .FirstOrDefault()?
                .Parse(photo);
        }
    }
}