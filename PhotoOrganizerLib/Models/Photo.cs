using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace PhotoOrganizerLib.Models
{
    /// <summary>This class represents an image and contains functionality to extract metadata.</summary>
    public class Photo
    {
        /// <summary>Gets and sets the hashtable containing the image's metadata.</summary>
        public Dictionary<string, object> ImageMetadata { get; set; }
        /// <summary> Gets the Image property of the object.</summary>
        public Image Image { get; }
        /// <summary>Gets and sets the name of the image.</summary>
        public string PhotoName { get; set; }
        /// <summary>Gets and set the absolute path to the folder containing the file (path/to/filefolder).</summary>
        public string AbsoluteFolderPath { get; set; }

        /// <summary>Gets the path to the Image using the absolute folder path and Image name.</summary>
        public string AbsolutePathToFile
        {
            get
            {
                return Path.Join(AbsoluteFolderPath, PhotoName);
            }
        }

        /// <summary>Initializing a new instance of the <see cref="PhotoOrganizerLib.Models.Photo" /> class.</summary>
        public Photo(string fileName, string absoluteDirectoryPath)
        {
            ImageMetadata = new Dictionary<string, object>();
            AbsoluteFolderPath = absoluteDirectoryPath;
            PhotoName = fileName;

            try
            {
                Image = Image.FromFile(AbsolutePathToFile);
            } 
            catch(Exception)
            {
                Image = null;
            }
        }

        /// <summary>Prints image metadata according to input key.</summary>
        /// <remarks>Only prints metadata if the key exists in the metadata hashtable. Otherwise only image name is printed.</remarks>
        /// <param name="exifKey">String of metadata key.</param>
        public void TryPrintSpecificExifData(string exifKey)
        {
            if (ImageMetadata.ContainsKey(exifKey))
            {
                Console.WriteLine("Image Name: {0}", PhotoName);
                Console.WriteLine("{0}: {1}", exifKey, ImageMetadata[exifKey]);
            }
        }

        /// <summary>Prints list of extracted image metadata.</summary>
        public void PrintArrayExifData()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("Image Name: {0}", PhotoName);

            foreach (string key in ImageMetadata.Keys)
            {
                Console.WriteLine("{0}: {1}", key, ImageMetadata[key]);
            }

            Console.WriteLine("======================================\n");
        }

        /// <summary>Adds key-value pair to the metadata dictionary.</summary>
        /// <param name="key">Name of the key for the entry.</param>
        /// <param name="value">Object containing the metadata.</param>
        public void AddMetadata(string key, object value)
        {
            ImageMetadata.Add(key, value);
        }
    }
}