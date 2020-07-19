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
    /// <summary>Class for parsing <see cref="Directory" /> to <see cref="Photo" /> metadata dictionary.</summary>
    public static class ParseMetadata
    {
        /// <summary>Parses the list of <see cref="Directory" /> and saves the metadata to the <see cref="Photo" /> object.</summary>
        /// <param name="photo">Picture object used for extracting metadata and saving the values.</param>
        /// <param name="directories">Enumerable of <see cref="Directory" />, containing the different image <see cref="Tag" />s.</param>
        public static void Parse(Photo photo, IEnumerable<Directory> directories)
        {
            if (directories is null)
            {
                // Nothing to do
                return;
            }
            
            // TODO: Maybe change to returning a Dictionary<string, object> instead of adding to photo?
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