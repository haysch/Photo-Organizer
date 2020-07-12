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

                foreach (var directory in Directory.EnumerateDirectories(currentDirectory))
                {
                    directoryQueue.Enqueue(directory);
                }

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