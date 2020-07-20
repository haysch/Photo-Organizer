using System;
using System.IO;

using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Enums;
using Microsoft.Extensions.Configuration;

namespace PhotoOrganizerLib.Services
{
    /// <summary>Renaming class for copying or moving of files.</summary>
    public class RenameService : IRenameService
    {
        private readonly RenameType _renameType;

        /// <summary>Constructor for renaming class. Sets up type used for renaming files.</summary>
        /// <param name="config">Configuration containing the <see cref="RenameType" /> value.</param>
        /// <remarks>Attempts to parse the rename type from the configuration.</remarks>
        /// <exception cref="ArgumentException">Unable to parse input <see cref="RenameType" />.</exception>
        public RenameService(IConfiguration config)
        {
            var renameTypeString = config.GetValue<string>("renameType");
            if (!Enum.TryParse(renameTypeString, true, out _renameType))
            {
                throw new ArgumentException($"Renaming with type '{ renameTypeString }' is not supported.");
            }
        }

        /// <summary>
        /// Finds a name for the input photo by using the Original DateTime information.
        /// </summary>
        /// <param name="photo">A <see cref="Photo" /> object containing the original date and time.</param>
        /// <param name="format">Format of the <see cref="DateTime" /> for the returned string.</param>
        /// <remarks>Uses only DateTimeOriginal for naming.</remarks>
        /// <returns>String containing date and time in provided format, or <see cref="string.Empty" /> if no Original DateTime information is available.</returns>
        /// <exception cref="FormatException">
        /// If the length of the format is 1 and it is not one of the format specifier characters defined in <see cref="System.Globalization.DateTimeFormatInfo" /> -or- format does not contain a valid custom pattern.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The range of the date and time is outside of the dates supported by the calender of the current culture.</exception>
        public string FindPhotoDateTime(Photo photo, string format)
        {
            if (photo.DateTimeOriginal.HasValue)
            {
                // throws error if ToString(format) fails
                // if the format is incorrect, it will propagate through the entire run => FAIL FAST!
                return photo.DateTimeOriginal.Value.ToString(format);
            }

            return string.Empty;
        }

        /// <summary>Renames file according to the <see cref="RenameType" />.</summary>
        /// <param name="sourcePath">Path to the source file.</param>
        /// <param name="targetPath">Path to target file.</param>
        /// <exception cref="ArgumentException">Thrown when the rename type is not valid.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the file to be renamed does not exist.</exception>
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
                    // case RenameType.Replace:
                    //     File.Replace(sourcePath, targetPath, sourcePath + ".backup");
                    //     return;
                    case RenameType.None:
                    default:
                        return;
                }
            }
            catch (FileNotFoundException)
            {
                // TODO log unable to rename file
            }
            catch (DirectoryNotFoundException)
            {
                // TODO log
            }
            catch (IOException)
            {
                // TODO log
            }
            catch (ArgumentNullException)
            {
                // TODO log
            }
            catch (ArgumentException)
            {
                // TODO log
            }
            catch (Exception)
            {
                /// Possible exceptions:
                /// UnauthorizedAccessException, NotSupportedException, PathTooLongException
                // TODO bail out? - we might not be able to recover
                // throw;
            }
        }
    }
}