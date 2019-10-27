using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace PhotoOrganizer.Models
{
    /// <summary>This class represents an image and contains functionality to extract metadata.</summary>
    public class ImageFile
    {
        /// <summary>Gets and sets the hashtable containing the image's metadata.</summary>
        public Dictionary<string, object> ImageMetadata { get; set; }
        /// <summary> Gets the Image property of the object.</summary>
        public Image ImageData { get; }
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

        /// <summary>Initializing a new instance of the <see cref="Models.ImageFile" /> class.</summary>
        public ImageFile(string fileName, string absoluteDirectoryPath)
        {
            ImageMetadata = new Dictionary<string, object>();
            AbsoluteFolderPath = absoluteDirectoryPath;
            ImageName = fileName;

            ImageData = Image.FromStream(File.OpenRead(AbsolutePathToFile));

            // byte[] byteArray;
            // using (var ms = new MemoryStream())
            // {
            //     ImageProperty.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //     byteArray = ms.ToArray();
            // }

            // string base64String = Convert.ToBase64String(byteArray);
            // byte[] dataArray = Encoding.Default.GetBytes(base64String);

            // File.WriteAllBytes(ImageName + ".txt", dataArray);
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