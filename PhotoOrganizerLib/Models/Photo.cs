using System;
using System.IO;
using System.Collections.Generic;

namespace PhotoOrganizerLib.Models
{
    /// <summary>This class represents an image and contains functionality to extract metadata.</summary>
    public class Photo
    {
        /// <summary>Gets and sets the hashtable containing the image's metadata.</summary>
        public Dictionary<string, object> ImageMetadata { get; set; }
        /// <summary>Gets and sets the name of the image.</summary>
        public string Name { get; set; }
        /// <summary>Gets and sets the directory path to the file excluding its name.</summary>
        public string DirectoryPath { get; set; }
        /// <summary>Gets and set the absolute file path to the photo.</summary>
        public string AbsoluteFilePath
        {
            get => Path.Join(DirectoryPath, Name);

            set
            {
                Name = Path.GetFileName(value);
                DirectoryPath = Path.GetDirectoryName(value);
            }
        }

        /// <summary>Initializing a new instance of the <see cref="PhotoOrganizerLib.Models.Photo" /> class.</summary>
        public Photo(string absoluteFilePath)
        {
            ImageMetadata = new Dictionary<string, object>();
            AbsoluteFilePath = absoluteFilePath;
            
        }

        /// <summary>Prints image metadata according to input key.</summary>
        /// <remarks>Only prints metadata if the key exists in the metadata hashtable. Otherwise only image name is printed.</remarks>
        /// <param name="exifKey">String of metadata key.</param>
        public void TryPrintSpecificExifData(string exifKey)
        {
            if (ImageMetadata.ContainsKey(exifKey))
            {
                Console.WriteLine("Image Name: {0}", Name);
                Console.WriteLine("{0}: {1}", exifKey, ImageMetadata[exifKey]);
            }
        }

        /// <summary>Prints list of extracted image metadata.</summary>
        public void PrintArrayExifData()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("Image Name: {0}", Name);

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