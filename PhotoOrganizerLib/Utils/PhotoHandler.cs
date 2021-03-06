using MetadataExtractor.Util;
using PhotoOrganizerLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PhotoOrganizerLib.Utils
{
    public static class PhotoHandler
    {
        /// <summary>
        /// Finds all valid image formats, <see cref="FileType" />, given some input path.
        /// Iterates through all subdirectories from the input path.
        /// </summary>
        /// <param name="path">Input path to begin searching</param>
        /// <returns>A list of <see cref="Photo" /></returns>
        /// <remarks>If the input path directory does not exist, return an empty list.</remarks>
        public static async IAsyncEnumerable<Photo> FindPhotosAsync(string path)
        {
            // Check if initial directory exists, otherwise we wouldn't have anything to do
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Directory { path } does not exist.");
                yield break;
            }

            var directoryQueue = new Queue<string>();
            directoryQueue.Enqueue(path); // push input path

            // Queue folders and find all photo files
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
                    var photo = await Task.Run(() => FindPhoto(filePath));

                    if (!(photo is null))
                    {
                        yield return photo;
                    }
                }
            }
        }

        /// <summary>
        /// Method for finding files that are supported by MetadataExtractor.
        /// </summary>
        /// <param name="filePath">Path to file to check.</param>
        /// <returns>Photo object of the <paramref name="filePath" />, or <see langword="null" /> if file type is not supported.</returns>
        private static Photo? FindPhoto(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            if (FileTypeDetector.DetectFileType(stream) != FileType.Unknown)
            {
                return new Photo(filePath);
            }

            return null;
        }
    }
}