using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Configuration;

using PhotoOrganizerLib.Extensions;
using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Models;

namespace PhotoOrganizerLib.Utils
{
    public class SortService : ISortService
    {
        private string _outputPath;
        private IRenameService _renameService;
        // REMARK currently only accepts DateTime with YYYY and MM (e.g. yyyy/mm/Photo.jpg)
        private Dictionary<string, HashSet<string>> _yearDirectories;
        private bool _unknownDirectoryExists;

        /// <summary>
        /// Service used for sorting files and <see cref="PhotoOrganizerLib.Models.Photo" /> using datetimes.
        /// </summary>
        public SortService(IConfiguration config, IRenameService renameService)
        {
            _outputPath = config["outputPath"];
            _yearDirectories = EnumerateDirectoryStructure(_outputPath);
            _unknownDirectoryExists = TryFindUnknownDirectory(_outputPath);

            _renameService = renameService;
        }

        /// <summary>Sort photo by extracting its DateTimeOriginal value.</summary>
        /// <param name="photo">A <see cref="PhotoOrganizerLib.Models.Photo" /> object.</param>
        public void SortPhoto(Photo photo) 
        {
            var sourcePath = photo.AbsolutePathToFile;
            var dateTimeString = _renameService.FindPhotoDateTime(photo, "yyyyMMdd_HHmmss");
            SortDateTime(sourcePath, dateTimeString);
        }

        /// <summary>
        /// Sorts file using `%OUTPUTPATH%/yyyy/MM/datetime.ext` structure based on the provided datetime string.
        /// If the datetime string is not a valid <see cref="System.DateTime" /> the image is placed in an `unknown/` folder.
        /// </summary>
        /// <param name="sourcePath">Path to file.</param>
        /// <param name="dateTimeString">String representation of a <see cref="System.DateTime" />.</param>
        /// <remarks>Sorting of unknown files are only allowed, if the `%OUTPUTPATH%/unknown/` directory exists or can be created.</remarks>
        public void SortDateTime(string sourcePath, string dateTimeString)
        {
            if (!DateTime.TryParse(dateTimeString, out var dateTime))
            {
                SortUnknownFile(sourcePath);
            }
            else
            {
                var fileExtension = Path.GetExtension(sourcePath);

                var year = dateTime.Year.ToString();
                var month = dateTime.Month.ToString();

                var sortPath = Path.Join(year, month, dateTimeString + fileExtension); // To make a bit more explicit
                var targetPath = Path.Join(_outputPath, sortPath);
                
                // If year and month directories don't exist, try to create target path
                // then rename file
                if ((_yearDirectories.TryGetValue(year, out var monthSet) && monthSet.Contains(month))
                    || TryCreateDirectory(targetPath))
                {
                    _renameService.RenameFile(sourcePath, targetPath);
                }
            }
        }

        /// <summary>
        /// Sorts file into `%OUTPUTPATH%/unknown/`, if it exists or can be created, using its current filename.
        /// </summary>
        /// <param name="sourcePath">Path to file.</param>
        private void SortUnknownFile(string sourcePath)
        {
            if (_unknownDirectoryExists)
            {
                var fileName = Path.GetFileName(sourcePath);
                var unknownPath = Path.Join(_outputPath, "unknown", fileName);
                _renameService.RenameFile(sourcePath, unknownPath);
            }
            // else don't do anything - just leave the image 
            // TODO: consider adding logging?
        }

        /// <summary>
        /// Enumerates the directory structure for the input path and saves the 
        /// `yyyy/MM` folder structure to a <cref name="System.Collection.Generic.Dictionary" /> for future reference when moving files.
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
                var yearString = directory.Substring(0,4);
                if (yearString.IsYear())
                {
                    var monthSet = new HashSet<string>();

                    var subDirs = Directory.EnumerateDirectories(directory);

                    foreach (var subDir in subDirs)
                    {
                        var monthString = subDir.Substring(0,2);
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
            var unknownPath = Path.Join(path, "unknown");
            return (Directory.Exists(unknownPath) || TryCreateDirectory(unknownPath));
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
                return false;
            }
        }
    }
}