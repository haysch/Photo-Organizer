using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoOrganizerLib.Enums;
using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Models;
using System;
using System.IO;

namespace PhotoOrganizerLib.Services
{
    /// <summary>Renaming class for copying or moving of files.</summary>
    public class RenameService : IRenameService
    {
        private readonly ILogger<IRenameService> _logger;
        private readonly RenameType _renameType;

        /// <summary>Constructor for renaming class. Sets up type used for renaming files.</summary>
        /// <param name="config">Configuration containing the <see cref="RenameType" /> value.</param>
        /// <remarks>Attempts to parse the rename type from the configuration.</remarks>
        /// <exception cref="ArgumentException">Unable to parse input <see cref="RenameType" />.</exception>
        public RenameService(ILogger<IRenameService> logger, IConfiguration config)
        {
            var renameTypeString = config.GetValue<string>("renameType");
            if (!Enum.TryParse(renameTypeString, true, out _renameType))
            {
                throw new ArgumentException($"Renaming with type '{ renameTypeString }' is not supported.");
            }

            _logger = logger;
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
        /// <param name="destPath">Path to target file.</param>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="NotSupportedException"><paramref name="sourcePath"/> or <paramref name="destPath"/> is in an invalid form.</exception>
        /// <exception cref="PathTooLongException">The specified path, or filename, or both exceeds the system-defined maximum length.</exception>
        public void RenameFile(string sourcePath, string destPath)
        {
            try
            {
                switch (_renameType)
                {
                    case RenameType.Copy:
                        File.Copy(sourcePath, destPath);
                        return;
                    case RenameType.Move:
                        File.Move(sourcePath, destPath);
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
                _logger.LogWarning($"Input file not found at { sourcePath }.");
            }
            catch (DirectoryNotFoundException)
            {
                _logger.LogWarning("Source or destination path not found.");
                _logger.LogDebug($"\tSource path: { sourcePath }\n\tDestination path: { destPath }");
            }
            catch (IOException)
            {
                _logger.LogWarning($"Destination file already exists at { destPath }.");
                _logger.LogDebug($"Source path: { sourcePath }");
            }
            catch (ArgumentNullException)
            {
                if (sourcePath is null)
                {
                    _logger.LogWarning("Source path is null.");
                    _logger.LogDebug($"Destination path: { destPath }");
                }
                else
                {
                    _logger.LogWarning("Destination path is null.");
                    _logger.LogDebug($"Source path: { sourcePath }");
                }
            }
            catch (ArgumentException)
            {
                _logger.LogWarning("Source or destination path is zero-length, contains only whitespace or contains invalid path characters.");
                _logger.LogWarning($"\tSource path: { sourcePath }\n\tDestination path: { destPath }");
            }
            catch (Exception ex)
            {
                /// Possible exceptions:
                /// UnauthorizedAccessException, NotSupportedException, PathTooLongException
                
                _logger.LogError($"{ typeof(IRenameService) } has encountered a problem which cannot be handled gracefully.\n{ ex }");
                throw;
            }
        }
    }
}