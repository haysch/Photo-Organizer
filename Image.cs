using System;
using System.Text;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using PhotoOrganizer.Primitives;
using PhotoOrganizer.Util;

namespace PhotoOrganizer {
    /// <summary>This class represents an image and contains functionality to extract metadata.</summary>
    public class Image {
        /// <summary>Gets the DateTimeOriginal of the image.</summary>
        public DateTime DateTimeOriginal { get; internal set; }
        /// <summary>Gets the ISO value of the camera.</summary>
        public ushort ISO { get; internal set; }   
        /// <summary>Gets the F-number of the camera.</summary>
        public string FNumber { get; internal set; }
        /// <summary>Gets the focal length of the camera.</summary>
        public double FocalLength { get; internal set; }
        /// <summary>Gets the shutter speed of the camera.</summary>
        public string ExifShutterSpeed { get; internal set; }
        /// <summary>Gets the make of the camera.</summary>
        public string Make { get; internal set; }
        /// <summary>Gets the model of the camera.</summary>
        public string Model { get; internal set; }
        /// <summary>Gets the image width (in pixels).</summary>
        public int Width { get; internal set; }
        /// <summary>Gets the image height (in pixels).</summary>
        public int Height { get; internal set; }
        /// <summary>Gets the longitude where the image was taken.</summary>
        public double Longitude { get; internal set; }
        /// <summary>Gets the latitude where the image was taken.</summary>
        public double Latitude { get; internal set; }
        /// <summary>Gets the hash algorithm used for computing the hash.</summary>
        public string HashAlgorithm { get; internal set; }
        /// <summary>Gets the computed hash value.</summary>
        public string HashValue { get; internal set; }

        private Bitmap image;
        private string filePath;
        /// <summary>Gets and sets the name of the image.</summary>
        public string ImageName { get; set; }

        // TODO add hashmap when initializing to get all propItems and call when needed ?
        /// <summary>Initializing a new instance of the <see cref="Image"/> class.</summary>
        public Image(string fileName, string absolutePath) {
            // TODO test deeper enumeration of path (subfolders)
            filePath = absolutePath + Path.DirectorySeparatorChar + fileName;
            image = new Bitmap(filePath);

            ImageName = fileName;
        }

        /// <summary> Extracts the DateTimeOriginal property from the image and converts it to DateTime format. </summary>
        /// <remarks> If the PropertyItem does not exist, assign DateTimeOriginal to UnixEpoch. </remarks>
        private void ExtractDateTimeOriginal() {
            try {
                var propItem = image.GetPropertyItem((int)PropertyTagId.ExifDTOrig);
                DateTimeOriginal = ExifDTToDateTime((string)PropertyTag.GetValue(propItem));
            } catch (Exception) {
                // if there is no value set to default
                DateTimeOriginal = DateTime.UnixEpoch;
            }
        }

        /// <summary> Extracts the ISO property from image and converts it to short string. </summary>
        /// <remarks> If the prop does not exist, assign variable to default value. </remarks>
        private void ExtractISO() {
            try {
                var propItem = image.GetPropertyItem((int)PropertyTagId.ISO);
                ISO = (ushort)PropertyTag.GetValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                ISO = 0;
            }
        }

        private void ExtractFNumber() {
            try {
                var propFNum = image.GetPropertyItem((int)PropertyTagId.FNumber);
                FNumber = ((Rational)PropertyTag.GetValue(propFNum)).ToFNumber();
            } catch (Exception) {
                FNumber = "";
            }
        }

        private void ExtractFocalLength() {
            try {
                var propItem = image.GetPropertyItem((int)PropertyTagId.FocalLength);
                FocalLength = (Rational)PropertyTag.GetValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                FocalLength = 0.0;
            }
        }

        private void ExtractMake() {
            try {
                var propItem = image.GetPropertyItem((int)PropertyTagId.EquipMake);
                Make = (string)PropertyTag.GetValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                Make = "";
            }
        }

        private void ExtractModel() {
            try {
                var propItem = image.GetPropertyItem((int)PropertyTagId.EquipModel);
                Model = (string)PropertyTag.GetValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                Model = "";
            }
        }

        private void ExtractResolution() {
            Width = image.Width;
            Height = image.Height;
        }
        
        /// <summary> Gets the latitude from the image in decimal format </summary>
        private void ExtractLatitude() {
            try {
                var propGpsLat = image.GetPropertyItem((int)PropertyTagId.Latitude);
                var propGpsLatRef = image.GetPropertyItem((int)PropertyTagId.GpsLatitudeRef);

                Rational[] latitudeDegMinSecs = (Rational[])PropertyTag.GetValue(propGpsLat);
                string GpsLatRef = (string)PropertyTag.GetValue(propGpsLatRef);

                Latitude = DegreesMinutesSecondsToDecimal(latitudeDegMinSecs, GpsLatRef);
            } catch (Exception) {
                // if there is no value set to default
                Latitude = 0;
            }
        }

        private void ExtractLongitude() {
            try {
                var propGpsLong = image.GetPropertyItem((int)PropertyTagId.Longitude);
                var propGpsLongRef = image.GetPropertyItem((int)PropertyTagId.GpsLongitudeRef);

                Rational[] longitudeDegMinSecs = (Rational[])PropertyTag.GetValue(propGpsLong);
                string GpsLongRef = (string)PropertyTag.GetValue(propGpsLongRef);

                Longitude = DegreesMinutesSecondsToDecimal(longitudeDegMinSecs, GpsLongRef);
            } catch (Exception) {
                // if there is no value set to default
                Longitude = 0;
            }
        }

        /// <summary>Private method for converting Degree/Minutes/Seconds to decimal degrees.</summary>
        /// <returns>Double of the coordinate.</returns>
        /// <remarks>If input array is not of size 3, return 0.0.</remarks>
        /// <param name="degMinSec">Rational array containing the Degree/Minutes/Seconds.</param>
        /// <param name="gpsRef">GPS reference specifying direction, e.g. "N" or "E".</param>
        private double DegreesMinutesSecondsToDecimal(Rational[] degMinSec, string gpsRef) {
            if (degMinSec.Length != 3) return 0;

            double hours = Math.Abs(degMinSec[0]);
            double minutes = degMinSec[1];
            double seconds = degMinSec[2];

            double value = hours + (minutes / 60.0d) + (seconds / 3600.0d);
            
            // If Ref is not N or E, negate the value.
            if (gpsRef == "S" || gpsRef == "W") value *= -1;
            return value;
        }

        /// <summary>Wrapper for computing the hash value of image. Sets <see cref="Image.HashAlgorithm"/> and <see cref="Image.HashValue"/>.</summary>
        /// <param name="algorithm">Algorithm used for computing hash. See <see cref="Util.Algorithm"/> for available hash algorithms</param>
        private void ComputeHash(Algorithm algorithm) {
            Checksum chksum = new Checksum(algorithm);
            HashAlgorithm = algorithm.ToString();
            HashValue = chksum.ComputeHash(filePath);
        }

        /// <summary>TODO move to own object / ImageMetadata</summary>
        public void ExtractMetadata() {
            PropertyItem[] propItems = image.PropertyItems;

            foreach (PropertyItem item in propItems) {
                switch ((PropertyTagId)item.Id) {
                    case PropertyTagId.ExifDTOrig:
                        DateTimeOriginal = ExifDTToDateTime((string)PropertyTag.GetValue(item));
                        //ExtractDateTimeOriginal();
                        continue;
                    case PropertyTagId.ISO:
                        ExtractISO();
                        continue;
                    case PropertyTagId.FNumber:
                        ExtractFNumber();
                        continue;
                    case PropertyTagId.ExifShutterSpeed: // TODO See PropertyTag -> Rational
                        ExifShutterSpeed = ((Rational)PropertyTag.GetValue(item)).SimplifyFraction();
                        continue;
                    case PropertyTagId.FocalLength:
                        ExtractFocalLength();
                        continue;
                    case PropertyTagId.EquipMake:
                        ExtractMake();
                        continue;
                    case PropertyTagId.EquipModel:
                        ExtractModel();
                        continue;
                    case PropertyTagId.Latitude:
                        ExtractLatitude();
                        continue;
                    case PropertyTagId.Longitude:
                        ExtractLongitude();
                        continue;
                    default:
                        continue;
                }
            }
            // ExtractDateTimeOriginal();
            // ExtractISO();
            // ExtractFNumber();
            // ExtractFocalLength();
            // ExtractMake();
            // ExtractModel();
            ExtractResolution();
            // ExtractLatitude();
            // ExtractLongitude();
            ComputeHash(Algorithm.MD5);
        }

        /// <summary>Converts the Exif date time string format to DateTime format.</summary>
        /// <returns>DateTime of DateTimeOriginal.</returns>
        /// <param name="dtOrig">String containing the DateTimeOriginal</param>
        private static DateTime ExifDTToDateTime(string dtOrig) {
            dtOrig = dtOrig.Replace(' ', ':');
            string[] ymdHms = dtOrig.Split(':');

            int year    = int.Parse(ymdHms[0]);
            int month   = int.Parse(ymdHms[1]);
            int day     = int.Parse(ymdHms[2]);
            int hour    = int.Parse(ymdHms[3]);
            int minute  = int.Parse(ymdHms[4]);
            int second  = int.Parse(ymdHms[5]);
            
            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}