using System;
using System.Drawing.Imaging;
using System.Collections.Generic;

using PhotoOrganizer.Primitives;
using PhotoOrganizer.Models;

namespace PhotoOrganizer.Util
{
    /// <summary>Class represents an extractor for extracting and assign values to the models of the ImageData class. See <see cref="ImageFile"/> for structure.</summary>
    public class Extractor
    {
        private int _extractCounter;
        private Checksum _checksum;

        /// <summary>
        /// Simple constructor for initializing an Extractor.
        /// Defaults checksum algorithm to MD5.
        /// </summary>
        public Extractor()
        {
            _checksum = new Checksum(Algorithm.MD5);
            _extractCounter = 0;
        }

        /// <summary>Initializing a new instance Extractor.</summary>
        /// <param name="hashAlgorithm">Algorithm used for computing hash. See <see cref="Primitives.Algorithm"/>for available hash algorithms</param>
        public Extractor(Algorithm hashAlgorithm)
        {
            _checksum = new Checksum(hashAlgorithm);
            _extractCounter = 0;
        }

        /// <summary>Method for extracting metadata from image file.</summary>
        /// <returns>Dictionary containing the extracted, <see cref="Primitives.PropertyTagId"/>, property items.</returns>
        /// <param name="image">ImageData class of image object. See <see cref="ImageFile"/> for structure.</param>
        public Dictionary<string, object> ExtractMetadata(ImageFile image)
        {
            Dictionary<string, object> metadata = new Dictionary<string, object>();
            PropertyItem[] propItems = image.ImageData.PropertyItems;

            _extractCounter++;

            foreach (PropertyItem item in propItems)
            {
                switch ((PropertyTagId)item.Id)
                {
                    case PropertyTagId.ExifDTOrig:
                    case PropertyTagId.DateTime:
                        metadata.Add(((PropertyTagId)item.Id).ToString(), ExifDTToDateTime((string)PropertyTag.GetValue(item)));
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
                            PropertyItem propGpsLatRef = image.ImageData.GetPropertyItem((int)PropertyTagId.GpsLatitudeRef);

                            // Convert from Deg/Min/Secs to decimal degrees
                            Rational[] latitudeDegMinSecs = (Rational[])PropertyTag.GetValue(item);
                            string GpsLatRef = (string)PropertyTag.GetValue(propGpsLatRef);

                            metadata.Add("Latitude", DegreesMinutesSecondsToDecimal(latitudeDegMinSecs, GpsLatRef));
                        }
                        continue;

                    case PropertyTagId.Longitude:
                        if ((PropertyTagType)item.Type == PropertyTagType.Rational)
                        {
                            PropertyItem propGpsLongRef = image.ImageData.GetPropertyItem((int)PropertyTagId.GpsLongitudeRef);

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
            metadata.Add("Width", width);
            metadata.Add("Height", height);

            string hashAlgo, hashValue;
            ComputeHash(image.AbsolutePathToFile, out hashAlgo, out hashValue);
            metadata.Add("HashAlgorithm", hashAlgo);
            metadata.Add("HashValue", hashValue);

            return metadata;
        }

        private void ExtractResolution(ImageFile image, out int width, out int height)
        {
            width = image.ImageData.Width;
            height = image.ImageData.Height;
        }

        /// <summary>Private method for converting Degree/Minutes/Seconds to decimal degrees.</summary>
        /// <returns>Double of the coordinate.</returns>
        /// <remarks>If input array is not of size 3, return 0.0.</remarks>
        /// <param name="degMinSec">Rational array containing the Degree/Minutes/Seconds.</param>
        /// <param name="gpsRef">GPS reference specifying direction, e.g. "N" or "E".</param>
        private double DegreesMinutesSecondsToDecimal(Rational[] degMinSec, string gpsRef)
        {
            if (degMinSec.Length != 3) 
                return 0;

            double hours = Math.Abs(degMinSec[0]);
            double minutes = degMinSec[1];
            double seconds = degMinSec[2];

            double value = hours + (minutes / 60.0d) + (seconds / 3600.0d);

            // If Ref is not N or E, negate the value.
            if (gpsRef == "S" || gpsRef == "W")
                value *= -1;
            
            return value;
        }

        /// <summary>Wrapper for computing the hash value of input image.</summary>
        /// <param name="imagePath">Path to the image.</param>
        /// <param name="hashAlgo">Out variable for the hash algorithm.</param>
        /// <param name="hashValue">Out variable for the hash value.</param>
        private void ComputeHash(string imagePath, out string hashAlgo, out string hashValue)
        {
            hashAlgo = _checksum.HashAlg;
            hashValue = _checksum.ComputeHash(imagePath);
        }

        /// <summary>Private method for converting the Exif DateTime string to DateTime format.</summary>
        /// <returns>DateTime object.</returns>
        /// <remarks>If the string format for yMd-hMs is malform, a standard value corresponding to 01/01/0001-00:00:01 will be selected.</remarks>
        /// <param name="dtOrig">String containing the Exif DateTime format.</param>
        private static DateTime ExifDTToDateTime(string dtOrig)
        {
            dtOrig = dtOrig.Replace(' ', ':');
            string[] ymdHms = dtOrig.Split(':');
            int year, month, day, hour, minute, second;

            if (ymdHms.Length == 6)
            {
                if (!Int32.TryParse(ymdHms[0], out year))
                    year = 1;
                if (!Int32.TryParse(ymdHms[1], out month))
                    month = 1;
                if (!Int32.TryParse(ymdHms[2], out day))
                    day = 1;
                if (!Int32.TryParse(ymdHms[3], out hour))
                    hour = 0;
                if (!Int32.TryParse(ymdHms[4], out minute))
                    minute = 0;
                if (!Int32.TryParse(ymdHms[5], out second))
                    second = 1;
            }
            else
            {
                year = 1;
                month = 1;
                day = 1;
                hour = 0;
                minute = 0;
                second = 1;
            }

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}