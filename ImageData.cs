using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace PhotoOrganizer
{
    /// <summary>This class represents an image and contains functionality to extract metadata.</summary>
    public class ImageData
    {
        /// TODO change structure to hashmap
        /// <summary>Gets and sets the hashtable containing the image's metadata.</summary>
        public Dictionary<string, object> ImageMetadata { get; set; }
        /// <summary>Gets the DateTimeOriginal of the image.</summary>
        public DateTime DateTimeOriginal { get; set; }
        /// <summary>Gets and sets the ISO value of the camera.</summary>
        public ushort ISO { get; set; }
        /// <summary>Gets and sets the F-number of the camera.</summary>
        public string FNumber { get; set; }
        /// <summary>Gets and sets the focal length of the camera.</summary>
        public double FocalLength { get; set; }
        /// <summary>Gets and sets the shutter speed of the camera.</summary>
        public string ExifShutterSpeed { get; set; }
        /// <summary>Gets and sets the make of the camera.</summary>
        public string Make { get; set; }
        /// <summary>Gets and sets the model of the camera.</summary>
        public string Model { get; set; }
        /// <summary>Gets and sets the image width (in pixels).</summary>
        public int Width { get; set; }
        /// <summary>Gets and sets the image height (in pixels).</summary>
        public int Height { get; set; }
        /// <summary>Gets and sets the longitude where the image was taken.</summary>
        public double Longitude { get; set; }
        /// <summary>Gets and sets the latitude where the image was taken.</summary>
        public double Latitude { get; set; }
        /// <summary>Gets and sets the hash algorithm used for computing the hash.</summary>
        public string HashAlgorithm { get; set; }
        /// <summary>Gets and sets the computed hash value.</summary>
        public string HashValue { get; set; }
        /// <summary> Gets the Image property of the object.</summary>
        public Image ImageProperty { get; }
        /// <summary>Gets and sets the name of the image.</summary>
        public string ImageName { get; set; }
        /// 
        public string AbsoluteFilePath;

        // TODO add hashmap when initializing to get all propItems and call when needed ?
        /// <summary>Initializing a new instance of the <see cref="ImageData"/> class.</summary>
        public ImageData(string fileName, string absoluteDirectoryPath)
        {
            ImageMetadata = new Dictionary<string, object>();
            AbsoluteFilePath = absoluteDirectoryPath + Path.DirectorySeparatorChar + fileName;
            ImageProperty = Image.FromStream(File.OpenRead(AbsoluteFilePath));

            ImageName = fileName;
        }

        /// <summary>Prints all information of the image.</summary>
        public void PrintImageInfo()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("Image Name: {0}", ImageName);
            foreach (string key in ImageMetadata.Keys)
                Console.WriteLine("{0}: {1}", key, ImageMetadata[key]);
            // Console.WriteLine("DateTimeOriginal: {0}", ((DateTime) ImageMetadata["DateTimeOriginal"]).ToString("dd/MM/yyyy HH:mm:ss"));
            // Console.WriteLine("ISO: {0}", ImageMetadata["ISO"]);
            // Console.WriteLine("F-Number: {0}", ImageMetadata["FNumber"]);
            // Console.WriteLine("Focal Length: {0}", ImageMetadata["FocalLength"]);
            // Console.WriteLine("Shutter Speed: {0}", ImageMetadata["ExifShutterSpeed"]);
            // Console.WriteLine("Make: {0}\tModel: {1}", ImageMetadata["Make"], ImageMetadata["Model"]);
            // Console.WriteLine("Width: {0}\tHeight: {1}", ImageMetadata["Width"], ImageMetadata["Height"]);
            // Console.WriteLine("Lat/Long: {0:N6}, {1:N6}", ImageMetadata["Latitude"], ImageMetadata["Longitude"]);
            // Console.WriteLine("Hash value ({1}): {0}", ImageMetadata["HashValue"], ImageMetadata["HashAlgorithm"]);
            Console.WriteLine("======================================\n");
        }

        /// <summary>Prints image metadata according to input key.</summary>
        /// <remarks>Only prints metadata if the key exists in the metadata hashtable. Otherwise only image name is printed.</remarks>
        /// <param name="key">String of metadata keys.</param>
        public void PrintSpecificExifData(string key)
        {
            Console.WriteLine("Image Name: {0}", ImageName);
            if (ImageMetadata.ContainsKey(key))
                Console.WriteLine("{0}: {1}", key, ImageMetadata[key]);
        }

        /// <summary>Prints list of image metadata.</summary>
        /// <remarks>Only prints metadata if the key exists in the metadata hashtable. Otherwise only image name is printed.</remarks>
        /// <param name="keys">Array of metadata keys as string.</param>
        public void PrintArrayExifData(string[] keys)
        {
            Console.WriteLine("======================================");
            Console.WriteLine("Image Name: {0}", ImageName);

            foreach (string key in keys)
            {
                if (ImageMetadata.ContainsKey(key))
                    Console.WriteLine("{0}: {1}", key, ImageMetadata[key]);
            }

            Console.WriteLine("======================================\n");
        }
    }
}