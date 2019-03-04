using System;
using System.IO;
using System.Collections.Generic;

using PhotoOrganizer.Util;
using PhotoOrganizer.DataModel;

namespace PhotoOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ImageData> imageList;

            string[] files = RetrieveFileList();

            imageList = LoadImages(files);

            ExtractImageInformation(imageList);

            // RenameImageFiles(imageList);
        }

        private static string[] RetrieveFileList()
        {
            Queue<string> directoryQueue = new Queue<string>();
            List<string> fileList = new List<string>();

            Console.Write("Enter full or relative path to working directory: ");
            string workingDirectory = "testdata";// TODO uncomment Console.ReadLine();

            while (true)
            {
                if (Directory.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + workingDirectory))
                { // Relative path
                    workingDirectory = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + workingDirectory;
                    break;
                }
                else if (Directory.Exists(workingDirectory))
                { // Full path
                    break;
                }

                Console.WriteLine("Path {0} does not exist.", workingDirectory);
                Console.Write("Try again: ");
                workingDirectory = Console.ReadLine();
            }

            directoryQueue.Enqueue(workingDirectory);

            // Queue subfolders and find all files
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

        private static List<ImageData> LoadImages(string[] fileList)
        {
            List<ImageData> imageList = new List<ImageData>();

            // TODO enumerate folders at i depth
            foreach (string file in fileList)
            {
                int fileSplitIndex = file.LastIndexOf('/');

                string fileName = file.Substring(fileSplitIndex + 1);
                string filePath = file.Substring(0, fileSplitIndex);

                string fileType = Path.GetExtension(file).ToLower();

                // TODO add fileType acceptance
                switch (fileType)
                {
                    // Supported filetypes
                    case ".jpg":
                    case ".jpeg":
                        // if (fileName == "img4.jpg")
                            imageList.Add(new ImageData(fileName, filePath));
                        continue;
                    // Not supported filetypes
                    default:
                        continue;
                }
            }

            return imageList;
        }

        private static void ExtractImageInformation(List<ImageData> imageList)
        {
            int imageCounter = 0;
            Extractor extractor = new Extractor(Algorithm.MD5);

            foreach (ImageData image in imageList)
            {
                image.ImageMetadata = extractor.ExtractMetadata(image);
                imageCounter++;

                Console.Write("\rExtracting: {0}/{1}", imageCounter, imageList.Count);
            }

            if (ShowOutput()) {
                foreach (ImageData image in imageList)
                    image.PrintArrayExifData();
            }
        }

        private static bool ShowOutput()
        {
            Console.Write("\nDo you want to show metadata from extraction? [y/N] ");
            string answer = Console.ReadLine();

            if (answer.ToUpper() == "Y")
                return true;

            return false;
        }

        private static void RenameImageFiles(List<ImageData> imageList)
        {
            RenameType type = RenameType.None;
            bool done = false;

            while (!done)
            {
                Console.Write("\nDo you want to [copy] or [move] files? (if blank, no renaming will be done) ");
                string renameType = Console.ReadLine();

                switch (renameType.ToUpper())
                {
                    case "":
                        done = true;
                        break;
                    case "COPY":
                        type = RenameType.Copy;
                        done = true;
                        break;
                    case "MOVE":
                        type = RenameType.Move;
                        done = true;
                        break;
                    default:
                        Console.WriteLine("Option {0} not supported.", renameType);
                        continue;
                }
            }

            Rename renamer = new Rename(type);

            // TODO atm only jpeg images are supported
            foreach (ImageData image in imageList)
            {
                renamer.RenameJpeg(image);
            }
        }
    }
}
