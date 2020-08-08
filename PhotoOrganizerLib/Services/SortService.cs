using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoOrganizerLib.Enums;
using PhotoOrganizerLib.Extensions;
using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace PhotoOrganizerLib.Services
{
    public class SortService : ISortService
    {
        private readonly ILogger<ISortService> _logger;
        private readonly string _outputPath;
        private readonly IRenameService _renameService;
        private readonly bool _unknownDirectoryExists;

        // Default values
        private readonly string UNKNOWN_PATH = "unknown";
        private const string DEFAULT_DATETIME_FORMAT = "yyyyMMdd_HHmmss";

        // REMARK currently only accepts DateTime with YYYY and MM (e.g. yyyy/mm/Photo.jpg)
        public Dictionary<string, HashSet<string>> OutputDirectories;

        /// <summary>
        /// Service used for sorting files and <see cref="Photo" /> using datetimes.
        /// </summary>
        public SortService(ILogger<ISortService> logger, IConfiguration config, IRenameService renameService)
        {
            _logger = logger;
            _renameService = renameService;

            // set output path to "output" argument or current directory
            _outputPath = config.GetValue<string>("output");
            if (_outputPath is null)
            {
                _logger.LogInformation("Output directory is null. Using current directory as output");
                _outputPath ??= Directory.GetCurrentDirectory();
            }
            else
            {
                _logger.LogDebug($"Using { _outputPath } as output directory");
            }

            // Enumerate directories in output path
            OutputDirectories = EnumerateDirectoryStructure(_outputPath);
            _unknownDirectoryExists = TryFindUnknownDirectory(_outputPath);
        }

        /// <summary>
        /// Sort photo by extracting its DateTimeOriginal value.
        /// </summary>
        /// <param name="photo">A <see cref="Photo" /> object.</param>
        public void SortPhoto(Photo photo, string dateTimeFormat = DEFAULT_DATETIME_FORMAT)
        {
            var sourcePath = photo.FilePath;
            var dateTimeString = _renameService.FindPhotoDateTime(photo, dateTimeFormat);
            SortDateTime(sourcePath, dateTimeString, dateTimeFormat);
        }

        /// <summary>
        /// Sorts file using `%OUTPUTPATH%/yyyy/MM/datetime.ext` structure based on the provided datetime string.
        /// If the datetime string is not a valid <see cref="DateTime" /> the image is placed in an `unknown/` folder.
        /// </summary>
        /// <param name="sourcePath">Path to file.</param>
        /// <param name="dateTimeString">String representation of a <see cref="DateTime" />.</param>
        /// <param name="dateTimeFormat">Format used for the DateTime string. Default is 'yyyyMMdd_HHmmss'.</param>
        /// <param name="provider">Provider of culture used.</param>
        /// <param name="dateTimeStyles">Formatting options used for customizing string parsing.</param>
        /// <remarks>
        /// Sorting of unknown files are only allowed, if the `%OUTPUTPATH%/unknown/` directory exists or can be created.
        /// </remarks>
        public void SortDateTime(string sourcePath,
            string dateTimeString,
            string dateTimeFormat = DEFAULT_DATETIME_FORMAT,
            CultureInfo? provider = null,
            DateTimeStyles dateTimeStyles = DateTimeStyles.None)
        {
            if (_renameService.RenameType == RenameType.None)
            {
                return;
            }

            // set provider to default invariant, if provider is null
            provider ??= CultureInfo.InvariantCulture;

            if (!DateTime.TryParseExact(dateTimeString, dateTimeFormat, provider, dateTimeStyles, out var dateTime))
            {
                SortUnknownFile(sourcePath);
            }
            else
            {
                var fileExtension = Path.GetExtension(sourcePath);

                var year = dateTime.Year.ToString();
                var month = dateTime.Month.ToString("D2");

                var sortPath = Path.Join(year, month, dateTimeString + fileExtension); // To make a bit more explicit
                var destPath = Path.Join(_outputPath, sortPath);

                // If year and month directories don't exist, try to create target path
                // then rename file
                if ((OutputDirectories.TryGetValue(year, out var monthSet) && monthSet.Contains(month)))
                {
                    _renameService.RenameFile(sourcePath, destPath);
                }
                else if (TryCreateDirectory(Path.GetDirectoryName(destPath)))
                {
                    // Add year and month to output directory dictionary for faster lookup
                    AddToOutputDirectories(year, month);

                    _renameService.RenameFile(sourcePath, destPath);
                }
            }
        }

        /// <summary>
        /// Sorts file into `%OUTPUTPATH%/unknown/`, if it exists or could be created, using its current filename.
        /// </summary>
        /// <param name="sourcePath">Path to file.</param>
        private void SortUnknownFile(string sourcePath)
        {
            if (_unknownDirectoryExists)
            {
                var fileName = Path.GetFileName(sourcePath);
                var unknownPath = Path.Join(_outputPath, UNKNOWN_PATH, fileName);
                _renameService.RenameFile(sourcePath, unknownPath);
            }
            // else don't do anything - just leave the image 
        }

        /// <summary>
        /// Enumerates the directory structure for the input path and saves the
        /// `yyyy/MM` folder structure to a <see cref="Dictionary{TKey, TValue}" /> for future reference when moving files.
        /// </summary>
        /// <param name="path">Base path containing the output folders.</param>
        /// <returns>Dictionary containing years as keys and a hashsets as values, which contains the months for a given year.</returns>
        /// <remarks>Only accepts `yyyy/MM` folder structure.</remarks>
        private Dictionary<string, HashSet<string>> EnumerateDirectoryStructure(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"{ path } could not be found.");
            }

            var directoryStructure = new Dictionary<string, HashSet<string>>();

            var directories = Directory.GetDirectories(path);

            foreach (var directory in directories)
            {
                var lastDirectorySeparatorYear = directory.LastIndexOf(Path.DirectorySeparatorChar);
                var yearString = directory.Substring(lastDirectorySeparatorYear + 1);
                if (yearString.IsYear())
                {
                    var monthSet = new HashSet<string>();

                    var subDirectories = Directory.EnumerateDirectories(directory);

                    foreach (var subDirectory in subDirectories)
                    {
                        var lastDirectorySeparatorMonth = subDirectory.LastIndexOf(Path.DirectorySeparatorChar);
                        var monthString = subDirectory.Substring(lastDirectorySeparatorMonth + 1);
                        monthString = $"{monthString:00}"; // Pad with left zero is necessary
                        if (monthString.IsMonth())
                        {
                            monthSet.Add(monthString);
                        }
                    }

                    directoryStructure.Add(yearString, monthSet);
                }
            }

            return directoryStructure;
        }

        /// <summary>
        /// Finds the directory for sorting unknown files.
        /// If not found, it attempts to create it.
        /// </summary>
        /// <param name="path">Base path that contains the unknown directory.</param>
        /// <returns>Indicates whether it was possible to find, or create, the unknown directory.</returns>
        private bool TryFindUnknownDirectory(string path)
        {
            if (_renameService.RenameType == RenameType.None)
            {
                return false;
            }

            var unknownPath = Path.Join(path, UNKNOWN_PATH);
            var unknownExists = Directory.Exists(unknownPath);
            if (!unknownExists)
            {
                _logger.LogInformation($"Unknown directory { unknownPath } does not exist. Creating directory.");
                unknownExists = TryCreateDirectory(unknownPath);
            }
            return unknownExists;
        }

        /// <summary>
        /// Attempts to create all directories in the given path.
        /// </summary>
        /// <param name="path">Path of the directory to be created.</param>
        /// <returns>Boolean specified whether the creation was succesful.</returns>
        private bool TryCreateDirectory(string path)
        {
            try
            {
                return Directory.CreateDirectory(path).Exists;
            }
            catch (Exception)
            {
                _logger.LogDebug($"Could not create directory at { path }.");
                return false;
            }
        }

        /// <summary>
        /// Adds year and month to OutputDirectories dictionary to allow for faster lookup,
        /// than trying to create directory each time.
        /// </summary>
        /// <param name="year">Year string to add as key.</param>
        /// <param name="month">Month string to add in month set.</param>
        /// <remarks>
        /// If dictionary already contains year, add month to its set. Otherwise, create new key-value.
        /// </remarks>
        private void AddToOutputDirectories(string year, string month)
        {
            if (OutputDirectories.TryGetValue(year, out var monthSet))
            {
                monthSet.Add(month);
            }
            else
            {
                OutputDirectories.Add(year, new HashSet<string> { month });
            }
        }
    }
}