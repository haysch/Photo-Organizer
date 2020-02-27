using System;
using System.IO;
using System.Collections.Generic;

using PhotoOrganizer.Util;
using PhotoOrganizer.Models;

using Newtonsoft.Json;
using MetadataExtractor;

namespace PhotoOrganizer
{
    class Program
    {
        static Config _configuration;

        static void Main(string[] args)
        {
            LoadConfiguration();

            string[] files = RetrieveFileList();

            if (files.Length == 0)
                return;

            List<Picture> pictureList = Loadpictures(files);

            if (pictureList.Count == 0)
                return;

            ExtractpictureInformation(pictureList);

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

        private static string[] RetrieveFileList()
        {
            string startDirectory = _configuration.WorkDir;

            // Check if working directory exists
            var directoryQueue = new Queue<string>();
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

            return fileList.ToArray();
        }

        private static List<Picture> Loadpictures(string[] fileList)
        {
            var pictureList = new List<Picture>();

            foreach (var file in fileList)
            {
                var fileSplitIndex = file.LastIndexOf('/');

                var filePath = file.Substring(0, fileSplitIndex);
                var fileName = file.Substring(fileSplitIndex + 1);

                var fileType = Path.GetExtension(file).ToLower();

                // TODO add fileType acceptance
                switch (fileType)
                {
                    // Supported filetypes
                    case ".jpg":
                    case ".jpeg":
                        pictureList.Add(new Picture(fileName, filePath));
                        continue;
                    // Unsupported filetypes
                    default:
                        continue;
                }
            }

            return pictureList;
        }

        private static void ExtractpictureInformation(List<Picture> pictureList)
        {
            var pictureCounter = 0;
            var checksum = new Checksum(_configuration.HashAlgorithm);

            foreach (var picture in pictureList)
            {
                var directories = ImageMetadataReader.ReadMetadata(picture.AbsolutePathToFile);
                pictureCounter++;

                // Parse the result from ImageMetadataReader and save them to the Picture object
                ParseMetadata.Parse(picture, directories);
                
                var hashValue = checksum.ComputeHash(picture.AbsolutePathToFile);
                picture.AddMetadata("HashAlgorithm", _configuration.HashAlgorithm);
                picture.AddMetadata("HashValue", hashValue);

                // Console.Write("\rExtracting: {0}/{1}", pictureCounter, pictureList.Count);
            }

            if (_configuration.TraceEnabled)
            {
                foreach (Picture picture in pictureList)
                    picture.PrintArrayExifData();
            }
        }

        private static void RenamePictures(List<Picture> pictureList)
        {
            
            var renameType = _configuration.RenameType.ToUpper();

            var type = renameType switch
            {
                "COPY" => RenameType.Copy,
                "MOVE" => RenameType.Move,
                "REPLACE" => RenameType.Replace,
                _ => RenameType.None
            };

            var renamer = new Rename(type);

            // Only JPEG pictures are supported at the moment
            // as it is the only format tested
            foreach (var picture in pictureList)
            {
                var fileExtension = Path.GetExtension(picture.ImageName);

                try
                {
                    switch (fileExtension.ToLower())
                    {
                        case ".jpg":
                        case ".jpeg":
                            renamer.RenameImage(picture);
                            continue;
                        default:
                            Console.WriteLine("picture format {0} not supported.", fileExtension); // TODO log error
                            continue;
                    }
                }
                // TODO log files not able to rename
                catch (ArgumentException err)
                {
                    if (err.Message != null)
                        Console.WriteLine("ArgumentException message: {0}", err.Message); // TODO log error
                    continue;
                }
                catch (FileNotFoundException err)
                {
                    if (err.Message != null)
                        Console.WriteLine("FileNotFoundException message: {0}", err.Message); // TODO log error
                    continue;
                }
                catch (IOException err)
                {
                    if (err.Message != null)
                        Console.WriteLine("IOException message: {0}", err.Message); // TODO log error
                    continue;
                }
            }
        }
    }
}
