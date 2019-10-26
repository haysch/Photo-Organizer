using System;
using System.IO;
using System.Collections.Generic;

using PhotoOrganizer.Util;
using PhotoOrganizer.Models;

using Newtonsoft.Json;

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

            List<ImageFile> imageList = LoadImages(files);

            if (imageList.Count == 0)
                return;

            ExtractImageInformation(imageList);

            RenameImageFiles(imageList);
        }

        private static void LoadConfiguration()
        {
            using (var reader = new StreamReader("config.json"))
            {
                string json = reader.ReadToEnd();
                _configuration = JsonConvert.DeserializeObject<Config>(json);
            }
        }

        private static string[] RetrieveFileList()
        {
            string startDirectory = _configuration.WorkDir;

            // Check if working directory exists
            Queue<string> directoryQueue = new Queue<string>();
            if (Directory.Exists(startDirectory))
            {
                directoryQueue.Enqueue(startDirectory);
            }
            else
            {
                Console.WriteLine($"Directory {startDirectory} does not exist.");
            }

            // Queue folders and find all files
            List<string> fileList = new List<string>();
            while (directoryQueue.Count != 0)
            {
                string currDirectory = directoryQueue.Dequeue();

                foreach (string path in Directory.EnumerateDirectories(currDirectory))
                {
                    directoryQueue.Enqueue(path);
                }

                foreach (string file in Directory.EnumerateFiles(currDirectory))
                {
                    fileList.Add(file);
                }
            }

            return fileList.ToArray();
        }

        private static List<ImageFile> LoadImages(string[] fileList)
        {
            List<ImageFile> imageList = new List<ImageFile>();

            foreach (string file in fileList)
            {
                int fileSplitIndex = file.LastIndexOf('/');

                string filePath = file.Substring(0, fileSplitIndex);
                string fileName = file.Substring(fileSplitIndex + 1);

                string fileType = Path.GetExtension(file).ToLower();

                // TODO add fileType acceptance
                switch (fileType)
                {
                    // Supported filetypes
                    case ".jpg":
                    case ".jpeg":
                        if (fileName == "img4.jpg")
                            imageList.Add(new ImageFile(fileName, filePath));
                        continue;
                    // Not supported filetypes
                    default:
                        continue;
                }
            }

            return imageList;
        }

        private static void ExtractImageInformation(List<ImageFile> imageList)
        {
            int imageCounter = 0;
            Extractor extractor = new Extractor(_configuration.HashAlgorithm);

            foreach (ImageFile image in imageList)
            {
                image.ImageMetadata = extractor.ExtractMetadata(image);
                imageCounter++;

                Console.Write("\rExtracting: {0}/{1}", imageCounter, imageList.Count);
            }

            if (_configuration.TraceEnabled)
            {
                foreach (ImageFile image in imageList)
                    image.PrintArrayExifData();
            }
        }

        private static void RenameImageFiles(List<ImageFile> imageList)
        {
            RenameType type;
            string renameType = _configuration.RenameType;

            switch (renameType.ToUpper())
            {
                case "COPY":
                    type = RenameType.Copy;
                    break;
                case "MOVE":
                    type = RenameType.Move;
                    break;
                case "REPLACE":
                    type = RenameType.Replace;
                    break;
                default:
                    if (_configuration.TraceEnabled)
                        Console.WriteLine("No renaming will be done.");
                    type = RenameType.None;
                    break;
            }

            Rename renamer = new Rename(type);

            // Only JPEG images are supported at the moment
            // as it is the only format tested
            foreach (ImageFile image in imageList)
            {
                string fileExtension = Path.GetExtension(image.ImageName);

                try
                {
                    switch (fileExtension)
                    {
                        case ".jpg":
                        case ".jpeg":
                            renamer.RenameImage(image);
                            continue;
                        default:
                            Console.WriteLine("Image format {0} not supported.", fileExtension); // TODO log error
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
