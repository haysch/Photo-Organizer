using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace PhotoOrganizer.Models
{
    /// <summary>This class represents an image and contains functionality to extract metadata.</summary>
    public class Picture
    {
        /// <summary>Gets and sets the hashtable containing the image's metadata.</summary>
        public Dictionary<string, object> ImageMetadata { get; set; }
        /// <summary> Gets the Image property of the object.</summary>
        public Image Image { get; }
        /// <summary>Gets and sets the name of the image.</summary>
        public string ImageName { get; set; }
        /// <summary>Gets and set the absolute path to the folder containing the file (path/to/filefolder).</summary>
        public string AbsoluteFolderPath { get; set; }

        /// <summary>Gets the path to the Image using the absolute folder path and Image name.</summary>
        public string AbsolutePathToFile
        {
            get
            {
                return Path.Join(AbsoluteFolderPath, ImageName);
            }
        }

        /// <summary>Initializing a new instance of the <see cref="Models.Picture" /> class.</summary>
        public Picture(string fileName, string absoluteDirectoryPath)
        {
            ImageMetadata = new Dictionary<string, object>();
            AbsoluteFolderPath = absoluteDirectoryPath;
            ImageName = fileName;

            Image = Image.FromFile(AbsolutePathToFile);
        }

        /// <summary>Prints image metadata according to input key.</summary>
        /// <remarks>Only prints metadata if the key exists in the metadata hashtable. Otherwise only image name is printed.</remarks>
        /// <param name="key">String of metadata keys.</param>
        public void PrintSpecificExifData(string key)
        {
            if (ImageMetadata.ContainsKey(key))
            {
                Console.WriteLine("Image Name: {0}", ImageName);
                Console.WriteLine("{0}: {1}", key, ImageMetadata[key]);
            }
            else
            {
                Console.WriteLine("Metadata does not exist.");
            }
        }

        /// <summary>Prints list of extracted image metadata.</summary>
        public void PrintArrayExifData()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("Image Name: {0}", ImageName);

            foreach (string key in ImageMetadata.Keys)
            {
                Console.WriteLine("{0}: {1}", key, ImageMetadata[key]);
            }

            Console.WriteLine("======================================\n");
        }

        public void AddMetadata(string key, object value)
        {
            ImageMetadata.Add(key, value);
        }
    }
}