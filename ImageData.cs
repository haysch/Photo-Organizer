using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace PhotoOrganizer
{
    /// <summary>This class represents an image and contains functionality to extract metadata.</summary>
    public class ImageData
    {
        /// <summary>Gets and sets the hashtable containing the image's metadata.</summary>
        public Dictionary<string, object> ImageMetadata { get; set; }
        /// <summary> Gets the Image property of the object.</summary>
        public Image ImageProperty { get; }
        /// <summary>Gets and sets the name of the image.</summary>
        public string ImageName { get; set; }
        /// <summary>Gets and set the absolute path to the folder containing the file (path/to/filefolder).</summary>
        public string AbsoluteFolderPath { get; set; }
        /// <summary>Gets the path to the Image using the absolute folder path and Image name.</summary>
        public string AbsolutePathToFile
        {
            get
            {
                return AbsoluteFolderPath + Path.DirectorySeparatorChar + ImageName;
            }
        }

        // TODO add hashmap when initializing to get all propItems and call when needed ?
        /// <summary>Initializing a new instance of the <see cref="ImageData"/> class.</summary>
        public ImageData(string fileName, string absoluteDirectoryPath)
        {
            ImageMetadata = new Dictionary<string, object>();
            AbsoluteFolderPath = absoluteDirectoryPath;
            ImageName = fileName;

            ImageProperty = Image.FromStream(File.OpenRead(AbsolutePathToFile));
        }

        /// <summary>Prints image metadata according to input key.</summary>
        /// <remarks>Only prints metadata if the key exists in the metadata hashtable. Otherwise only image name is printed.</remarks>
        /// <param name="key">String of metadata keys.</param>
        public void PrintSpecificExifData(string key)
        {
            Console.WriteLine("Image Name: {0}", ImageName);
            if (ImageMetadata.ContainsKey(key))
                Console.WriteLine("{0}: {1}", key, ImageMetadata[key]);
            else
                Console.WriteLine("Metadata does not exist.");
        }

        /// <summary>Prints list of extracted image metadata.</summary>
        public void PrintArrayExifData()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("Image Name: {0}", ImageName);

            foreach (string key in ImageMetadata.Keys)
            {
                if (ImageMetadata.ContainsKey(key))
                    Console.WriteLine("{0}: {1}", key, ImageMetadata[key]);
            }

            Console.WriteLine("======================================\n");
        }
    }
}