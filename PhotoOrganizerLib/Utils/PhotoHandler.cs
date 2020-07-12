using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetadataExtractor.Util;
using PhotoOrganizerLib.Models;

namespace PhotoOrganizerLib.Utils
{
    public static class PhotoHandler
    {
        /// <summary>
        /// Finds all valid image formats, <see cref="MetadataExtractor.Util.FileType" />, given some input path.
        /// Iterates through all subdirectories from the input path.
        /// </summary>
        /// <param name="path">Input path to begin searching</param>
        /// <returns>A list of <see cref="PhotoOrganizerLib.Models.Photo" /></returns>
        /// <remarks>If the input path directory does not exist, return an empty list.</remarks>
        public static IEnumerable<Photo> FindPhotos(string path)
        {
            // Check if initial directory exists, otherwise we wouldn't have anything to do
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Directory { path } does not exist.");
                return Enumerable.Empty<Photo>();
            }

            var directoryQueue = new Queue<string>();
            directoryQueue.Enqueue(path); // push input path

            // Queue folders and find all photo files
            var photoList = new List<Photo>();
            while (directoryQueue.Count != 0)
            {
                // pop directory from queue
                var currentDirectory = directoryQueue.Dequeue();

                // enqueue all subdirectories in current directory
                foreach (var directory in Directory.EnumerateDirectories(currentDirectory))
                {
                    directoryQueue.Enqueue(directory);
                }

                // add all valid image formats to the photo list
                // see MetadataExtractor.Util.FileType for valid image formats
                foreach (var filePath in Directory.EnumerateFiles(currentDirectory))
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        if (FileTypeDetector.DetectFileType(stream) != FileType.Unknown)
                        {
                            photoList.Add(new Photo(filePath));
                        }
                    }
                }
            }

            return photoList;
        }
    }
}