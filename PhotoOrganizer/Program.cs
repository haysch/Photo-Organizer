using System;
using System.IO;
using System.Collections.Generic;

using PhotoOrganizer.Models;

using PhotoOrganizerLib.Utils;
using PhotoOrganizerLib.Models;

using Newtonsoft.Json;
using MetadataExtractor;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace PhotoOrganizer
{
    class Program
    {
        static Config _configuration;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"{System.IO.Directory.GetCurrentDirectory()}/appsettings.json", false) // TODO is there any point in using a settings file?
                .AddEnvironmentVariables("PHORG_")
                .AddCommandLine(args);

            var files = RetrieveFileList();

            var photoList = LoadPhotos(files);

            ExtractPhotoInformation(photoList);

            // RenamePictures(pictureList);
        }

        private static void LoadConfiguration()
        {
            using (var reader = new StreamReader("config.json"))
            {
                var json = reader.ReadToEnd();
                _configuration = JsonConvert.DeserializeObject<Config>(json);
            }
        }

        private static IEnumerable<string> RetrieveFileList()
        {
            var startDirectory = _configuration.WorkDir;
            var directoryQueue = new Queue<string>();

            // Check if working directory exists, otherwise we wouldn't have anything to do
            if (System.IO.Directory.Exists(startDirectory))
            {
                directoryQueue.Enqueue(startDirectory);
            }
            else
            {
                Console.WriteLine($"Directory {startDirectory} does not exist.");
            }

            // Queue folders and find all files
            var fileList = new List<string>();
            while (directoryQueue.Count != 0)
            {
                var currDirectory = directoryQueue.Dequeue();

                foreach (var path in System.IO.Directory.EnumerateDirectories(currDirectory))
                {
                    directoryQueue.Enqueue(path);
                }

                foreach (var file in System.IO.Directory.EnumerateFiles(currDirectory))
                {
                    fileList.Add(file);
                }
            }

            return fileList;
        }

        private static IEnumerable<Photo> LoadPhotos(IEnumerable<string> fileList)
        {
            var photoList = new List<Photo>();

            if (fileList.Count() == 0)
                return photoList;

            foreach (var file in fileList)
            {
                // finds the filename using the file path
                var fileSplitIndex = file.LastIndexOf('/');

                // splits the filepath into path and filename
                var filePath = file.Substring(0, fileSplitIndex);
                var fileName = file.Substring(fileSplitIndex + 1);

                // determines the file type from the extension (TODO might not be precise enough later on)
                var fileType = Path.GetExtension(file).ToLower();

                // TODO check might not be needed after implementing MetadataExtractor
                switch (fileType)
                {
                    // Supported filetypes
                    case ".jpg":
                    case ".jpeg":
                        photoList.Add(new Photo(fileName, filePath));
                        continue;
                    // Unsupported filetypes
                    default:
                        continue;
                }
            }

            return photoList;
        }

        private static void ExtractPhotoInformation(IEnumerable<Photo> photoList)
        {
            if (photoList.Count() == 0)
            {
                Console.WriteLine("No photos to extract data from.");
                return;
            }

            var pictureCounter = 0;
            var checksum = new Checksum(_configuration.HashAlgorithm);

            foreach (var photo in photoList)
            {
                var directories = ImageMetadataReader.ReadMetadata(photo.AbsolutePathToFile);
                pictureCounter++;

                // Parse the result from ImageMetadataReader and save them to the Picture object
                ParseMetadata.Parse(photo, directories);
                
                // Photo checksum
                var hashValue = checksum.ComputeChecksum(photo.AbsolutePathToFile);
                photo.AddMetadata("HashAlgorithm", _configuration.HashAlgorithm);
                photo.AddMetadata("HashValue", hashValue);

                // Dimensions
                photo.AddMetadata("Height", photo.Image.Height);
                photo.AddMetadata("Width", photo.Image.Width);

                // Console.Write("\rExtracting: {0}/{1}", pictureCounter, pictureList.Count);
            }
        }

        // private static void RenamePhotos(IEnumerable<Photo> photoList)
        // {
        //     var renameType = _configuration.RenameType;

        //     var renamer = new Rename(renameType);

        //     // TODO Only JPEG pictures are supported at the moment
        //     // as it is the only format tested
        //     foreach (var photo in photoList)
        //     {
        //         var fileExtension = Path.GetExtension(photo.PhotoName);

        //         try
        //         {
        //             // TODO remove check after implementing MetadataExtractor
        //             switch (fileExtension.ToLower())
        //             {
        //                 case ".jpg":
        //                 case ".jpeg":
        //                     renamer.RenamePhoto(photo);
        //                     continue;
        //                 default:
        //                     Console.WriteLine("picture format {0} not supported.", fileExtension); // TODO log error
        //                     continue;
        //             }
        //         }
        //         // TODO log files not able to rename
        //         catch (ArgumentException err)
        //         {
        //             if (err.Message != null)
        //                 Console.WriteLine("ArgumentException message: {0}", err.Message); // TODO log error
        //             continue;
        //         }
        //         catch (FileNotFoundException err)
        //         {
        //             if (err.Message != null)
        //                 Console.WriteLine("FileNotFoundException message: {0}", err.Message); // TODO log error
        //             continue;
        //         }
        //         catch (IOException err)
        //         {
        //             if (err.Message != null)
        //                 Console.WriteLine("IOException message: {0}", err.Message); // TODO log error
        //             continue;
        //         }
        //     }
        // }
    }
}
