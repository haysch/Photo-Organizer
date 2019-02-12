using System;
using System.Drawing.Imaging;
using System.Collections.Generic;

using PhotoOrganizer.Primitives;
using PhotoOrganizer.Util;

namespace PhotoOrganizer.Util
{
    /// <summary>Class represents an extractor for extracting and assign values to the models of the ImageData class. See <see cref="ImageData"/> for structure.</summary>
    public class Extractor
    {
        private int _extractCounter;
        /// <summary>Initializing a new instance Extractor.</summary>
        public Extractor()
        {
            _extractCounter = 0;
        }

        /// <summary>Method for extracting metadata from image file.</summary>
        /// <returns>Dictionary containing the extracted, <see cref="Primitives.PropertyTagId"/>, property items.</returns>
        /// <param name="image">ImageData class of image object. See <see cref="ImageData"/> for structure.</param>
        /// <param name="hashAlgorithm">Hashing algorithm to be used for computing the hash value of the image.</param>
        public Dictionary<string, object> ExtractMetadata(ImageData image, Algorithm hashAlgorithm)
        {
            Dictionary<string, object> metadata = new Dictionary<string, object>();
            PropertyItem[] propItems = image.ImageProperty.PropertyItems;

            _extractCounter++;

            foreach (PropertyItem item in propItems)
            {
                switch ((PropertyTagId)item.Id)
                {
                    case PropertyTagId.ExifDTOrig:
                    case PropertyTagId.DateTime:
                        metadata.TryAdd("DateTimeOriginal", ExifDTToDateTime((string)PropertyTag.GetValue(item)));
                        continue;

                    case PropertyTagId.ISO:
                        metadata.Add("ISO", (ushort)PropertyTag.GetValue(item));
                        continue;

                    case PropertyTagId.FNumber:
                        metadata.Add("FNumber", ((Rational)PropertyTag.GetValue(item)).ToFNumber());
                        continue;

                    case PropertyTagId.ExifShutterSpeed: // TODO See Util.PropertyTag -> Rational
                        metadata.Add("ExifShutterSpeed", ((Rational)PropertyTag.GetValue(item)).SimplifyFraction());
                        continue;

                    case PropertyTagId.FocalLength:
                        metadata.Add("FocalLength", ((Rational)PropertyTag.GetValue(item)).ToDouble());
                        continue;

                    case PropertyTagId.EquipMake:
                        metadata.Add("Make", (string)PropertyTag.GetValue(item));
                        continue;

                    case PropertyTagId.EquipModel:
                        metadata.Add("Model", (string)PropertyTag.GetValue(item));
                        continue;

                    case PropertyTagId.Latitude:
                        if ((PropertyTagType)item.Type == PropertyTagType.Rational)
                        {
                            PropertyItem propGpsLatRef = image.ImageProperty.GetPropertyItem((int)PropertyTagId.GpsLatitudeRef);

                            // Convert from Deg/Min/Secs to decimal degrees
                            Rational[] latitudeDegMinSecs = (Rational[])PropertyTag.GetValue(item);
                            string GpsLatRef = (string)PropertyTag.GetValue(propGpsLatRef);

                            metadata.Add("Latitude", DegreesMinutesSecondsToDecimal(latitudeDegMinSecs, GpsLatRef));
                        }
                        continue;

                    case PropertyTagId.Longitude:
                        if ((PropertyTagType)item.Type == PropertyTagType.Rational)
                        {
                            PropertyItem propGpsLongRef = image.ImageProperty.GetPropertyItem((int)PropertyTagId.GpsLongitudeRef);

                            // Convert from Deg/Min/Secs to decimal degrees
                            Rational[] longitudeDegMinSecs = (Rational[])PropertyTag.GetValue(item);
                            string GpsLongRef = (string)PropertyTag.GetValue(propGpsLongRef);

                            metadata.Add("Longitude", DegreesMinutesSecondsToDecimal(longitudeDegMinSecs, GpsLongRef));
                        }
                        continue;

                    default:
                        continue;
                }
            }

            int width, height;
            ExtractResolution(image, out width, out height);
            metadata.TryAdd("Width", width);
            metadata.TryAdd("Height", height);

            string hashAlgo, hashValue;
            ComputeHash(hashAlgorithm, image.AbsoluteFilePath, out hashAlgo, out hashValue);
            metadata.TryAdd("Hash Algorithm", hashAlgo);
            metadata.TryAdd("Hash Value", hashValue);

            return metadata;
        }

        private void ExtractResolution(ImageData image, out int width, out int height)
        {
            width = image.ImageProperty.Width;
            height = image.ImageProperty.Height;
        }

        /// <summary>Private method for converting Degree/Minutes/Seconds to decimal degrees.</summary>
        /// <returns>Double of the coordinate.</returns>
        /// <remarks>If input array is not of size 3, return 0.0.</remarks>
        /// <param name="degMinSec">Rational array containing the Degree/Minutes/Seconds.</param>
        /// <param name="gpsRef">GPS reference specifying direction, e.g. "N" or "E".</param>
        private double DegreesMinutesSecondsToDecimal(Rational[] degMinSec, string gpsRef)
        {
            if (degMinSec.Length != 3) return 0;

            double hours = Math.Abs(degMinSec[0]);
            double minutes = degMinSec[1];
            double seconds = degMinSec[2];

            double value = hours + (minutes / 60.0d) + (seconds / 3600.0d);

            // If Ref is not N or E, negate the value.
            if (gpsRef == "S" || gpsRef == "W") value *= -1;
            return value;
        }

        /// <summary>Wrapper for computing the hash value of input image.</summary>
        /// <param name="hashAlgorithm">Algorithm used for computing hash. See <see cref="Util.Algorithm"/> for available hash algorithms</param>
        /// <param name="imagePath">Path to the image.</param>
        /// <param name="hashAlgo">Out variable for the hash algorithm.</param>
        /// <param name="hashValue">Out variable for the hash value.</param>
        private void ComputeHash(Algorithm hashAlgorithm, string imagePath, out string hashAlgo, out string hashValue)
        {
            Checksum chksum = new Checksum(hashAlgorithm);
            hashAlgo = hashAlgorithm.ToString();
            hashValue = chksum.ComputeHash(imagePath);
        }

        /// <summary>Private method for converting the Exif DateTime string to DateTime format.</summary>
        /// <returns>DateTime object.</returns>
        /// <param name="dtOrig">String containing the Exif DateTime format.</param>
        private static DateTime ExifDTToDateTime(string dtOrig)
        {
            dtOrig = dtOrig.Replace(' ', ':');
            string[] ymdHms = dtOrig.Split(':');

            int year = int.Parse(ymdHms[0]);
            int month = int.Parse(ymdHms[1]);
            int day = int.Parse(ymdHms[2]);
            int hour = int.Parse(ymdHms[3]);
            int minute = int.Parse(ymdHms[4]);
            int second = int.Parse(ymdHms[5]);

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}