using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Configuration;

using PhotoOrganizerLib.Extensions;
using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Models;

namespace PhotoOrganizerLib.Utils
{
    public class Sort : ISort
    {
        private string _outputPath;
        private IRename _renameService;
        // TODO currently only accepts DateTime with YYYY and MM (e.g. yyyy/mm/Photo.jpg)
        private Dictionary<string, HashSet<string>> _yearDirectories;
        private bool _unknownDirectoryExists;

        public Sort(IConfiguration config, IRename renameService)
        {
            _outputPath = config["outputPath"];
            _yearDirectories = EnumerateDirectoryStructure(_outputPath);
            _unknownDirectoryExists = TryFindUnknownDirectory(_outputPath);

            _renameService = renameService;
        }

        public void SortPhoto(Photo photo) {}

        
        public void SortDateTimeFile(string sourcePath, string newFileName)
        {
            var fileExtension = Path.GetFileNameWithoutExtension(sourcePath);

            if (!DateTime.TryParse(newFileName, out var nameDt))
            {
                SortUnknownFile(sourcePath);
            }
            else
            {
                var year = nameDt.Year.ToString();
                var month = nameDt.Month.ToString();

                var sortPath = Path.Join(year, month, newFileName + fileExtension); // To make a bit more explicit
                var targetPath = Path.Join(_outputPath, sortPath);
                
                if (_yearDirectories.TryGetValue(year, out var monthSet) && monthSet.Contains(month))
                {
                    _renameService.RenameFile(sourcePath, targetPath);
                }
                else
                {
                    if (TryCreateDirectory(targetPath))
                    {
                        _renameService.RenameFile(sourcePath, targetPath);
                    }
                }
            }
        }

        public void SortUnknownFile(string sourcePath)
        {
            if (_unknownDirectoryExists)
            {
                var fileName = Path.GetFileName(sourcePath);
                var unknownPath = Path.Join(_outputPath, "unknown", fileName);
                _renameService.RenameFile(sourcePath, unknownPath);
            }
        }

        /// <summary>
        /// Enumerates the directory structure for the input path and saves the 
        /// YYYY/MM structure to a <cref name="System.Collection.Generic.Dictionary" /> for future reference when moving files.
        /// </summary>
        /// <param name="path">Base path containing the output folders.</param>
        /// <remarks>Only accepts YYYY/MM directory structure.</remarks>
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
                var year = directory.Substring(0,4);
                if (year.IsYear())
                {
                    var monthSet = new HashSet<string>();

                    var subDirs = Directory.EnumerateDirectories(directory);

                    foreach (var subDir in subDirs)
                    {
                        var month = subDir.Substring(0,2);
                        if (month.IsMonth())
                        {
                            monthSet.Add(month);
                        }
                    } 

                    directoryStructure.Add(year, monthSet);
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
        /// Moves file from sourcePath to targetPath with filename.
        /// </summary>
        /// <param name="sourcePath">Path containing the file.</param>
        /// <param name="targetPath">Target output path.</param>
        /// <param name="filename">Name of file to be moved.</param>
        /// <remarks>Moves file and keeps the same name.</remarks>
        private void MoveFile(string sourcePath, string targetPath, string filename)
        {
            var absoluteSourcePath = Path.Join(sourcePath, filename);
            var absoluteTargetPath = Path.Join(targetPath, filename);
            
            try
            {
                File.Move(absoluteSourcePath, absoluteTargetPath);
            }
            catch (FileNotFoundException)
            {
                // TODO log unable to move file and continue.
            }
            catch (Exception)
            {
                // TODO log unable to move file and stop - we might not be able to recover if the directory does not exist.
            }
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