using System;
using System.IO;

using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Enums;
using Microsoft.Extensions.Configuration;

namespace PhotoOrganizerLib.Utils
{
    /// <summary>Renaming class for copying or moving of files.</summary>
    public class RenameService : IRenameService
    {
        private RenameType _renameType;

        /// <summary>Constructor for renaming class. Sets up type used for renaming files.</summary>
        /// <param name="renameType">Type of move used for renaming. See <see cref="PhotoOrganizerLib.Enums.RenameType" /> for available types.</param>
        /// <remarks>Attempts to parse the rename type from the configuration.</remarks>
        /// <exception name="System.ArgumentException">Unable to parse input <see cref="PhotoOrganizerLib.Enums.RenameType" />.</exception>
        public RenameService(IConfiguration config)
        {
            if (!Enum.TryParse(config["renameType"], out _renameType))
            {
                throw new ArgumentException($"Rename Type { config["renameType"] } is invalid.");
            }
        }

        /// <summary>Finds a name for the input photo by using the Original DateTime information.</summary>
        /// <param name="photo">A <see cref="PhotoOrganizerLib.Models.Photo" />.</param>
        /// <param name="format">Format of the returned DateTime string.</param>
        /// <remarks>Uses only Original DateTime for naming.</remarks>
        /// <returns>Photo name in provided format, or <see cref="System.String.Empty"> if no Original DateTime information is available.</returns>
        public string FindDateTimeName(Photo photo, string format)
        {
            if (photo.ImageMetadata.ContainsKey("ExifDTOrig") &&
                DateTime.TryParse(photo.ImageMetadata["ExifDTOrig"] as string, out var photoDt))
            {
                return photoDt.ToString(format); // "yyyyMMdd_HHmmss"
            }

            return string.Empty;
        }

        /// <summary>Renames file according to the <see cref="PhotoOrganizerLib.Enums.RenameType" />.</summary>
        /// <param name="sourcePath">Path to the source file.</param>
        /// <param name="targetPath">Path to target file.</param>
        /// <exception cref="System.ArgumentException">Thrown when the rename type is not valid.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when the file to be renamed does not exist.</exception>
        public void RenameFile(string sourcePath, string targetPath)
        {
            try
            {
                switch (_renameType)
                {
                    case RenameType.Copy:
                        File.Copy(sourcePath, targetPath);
                        return;
                    case RenameType.Move:
                        File.Move(sourcePath, targetPath);
                        return;
                    case RenameType.Replace:
                        File.Replace(sourcePath, targetPath, sourcePath + ".backup");
                        return;
                    case RenameType.None:
                    default:
                        return;
                }
            }
            catch (Exception)
            {
                // TODO log unable to rename file
            }
        }
    }
}