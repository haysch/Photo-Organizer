using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using PhotoOrganizer.Util;

namespace PhotoOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            Rename renamer = new Rename(RenameType.Copy);
            List<ImageData> imageList;

            // Hashtable hashtable = new Hashtable();

            // hashtable.Add("DateTimeOriginal", DateTime.Now);
            // hashtable.Add("ISO", 640);
            // hashtable.Add("FocalLength", 6.6);

            // foreach (DictionaryEntry entry in hashtable)
            // {
            //     Console.WriteLine("{0} {1}", entry.Key, entry.Value);
            // }

            string[] files = RetrieveFileList();

            imageList = LoadImages(files);

            ExtractImageInformation(imageList);
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
                        //if (fileName == "img4.jpg")
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
            Extractor extractor = new Extractor();

            foreach (ImageData image in imageList)
            {
                image.ImageMetadata = extractor.ExtractMetadata(image, Algorithm.MD5);
                imageCounter++;

                // TODO add method for loading image dictionaries into database model
                // TODO add call for renaming of file

                Console.WriteLine("{0}/{1}", imageCounter, imageList.Count);
                image.PrintArrayExifData();
            }
        }
    }
}
