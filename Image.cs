using System;
using System.Text;
using System.Drawing;
using System.IO;

namespace PhotoOrganizer {
    class Image {
        public string DateTimeOriginal { get; internal set; }
        public ushort ISO { get; internal set; }   
        public double Aperture { get; internal set; }
        public string FNumber { get; internal set; }
        public double ShutterSpeed { get; internal set; }
        public double FocalLength { get; internal set; }
        public string Make { get; internal set; }
        public string Model { get; internal set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public double Longitude { get; internal set; }
        public double Latitude { get; internal set; }
        public double Altitude { get; internal set; }

        public int HashValue { get; internal set; }

        private Bitmap image;
        private string filePath;

        // TODO add hashmap when initializing to get all propItems and call when needed ?
        public Image(string imgName, string path) {
            // TODO test deeper enumeration of path (subfolders)
            filePath = path + Path.DirectorySeparatorChar + imgName;
            image = new Bitmap(filePath);
            
            // var propItems = img.PropertyItems;

            // foreach (var item in propItems) {
            //     Console.WriteLine(item.Id);
            // }
        }

        /// <summary> Extracts the DateTimeOriginal property from image and converts it to ASCII string. </summary>
        /// <remarks> If the prop does not exist, assign variable to default value. </remarks>
        private void ExtractDateTimeOriginal() {
            try {
                var propItem = image.GetPropertyItem((int)TagConstants.DTOrig);
                var dtOrig = (string)PropertyTag.getValue(propItem);
                DateTimeOriginal = ExifDTToDateTime(dtOrig);
            } catch (Exception) {
                // if there is no value set to default
                DateTimeOriginal = "";
            }
        }

        /// <summary> Extracts the ISO property from image and converts it to short string. </summary>
        /// <remarks> If the prop does not exist, assign variable to default value. </remarks>
        private void ExtractISO() {
            try {
                var propItem = image.GetPropertyItem((int)TagConstants.ISO);
                ISO = (ushort)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                ISO = 0;
            }
        }

        private void ExtractAperture() {
            try {
                var propItem = image.GetPropertyItem((int)TagConstants.Aperture);
                Aperture = (Rational)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                Aperture = 0.0;
            }
        }

        private void ExtractFNumber() {
            try {
                var propFNum = image.GetPropertyItem((int)TagConstants.FNumber);
                FNumber = ((Rational)PropertyTag.getValue(propFNum)).ToFNumber();
            } catch (Exception) {
                FNumber = "";
            }
        }

        private void ExtractShutterSpeed() {
            try {
                var propItem = image.GetPropertyItem((int)TagConstants.ShutterSpeed);
                ShutterSpeed = (Rational)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                ShutterSpeed = 0.0;
            }
        }

        private void ExtractFocalLength() {
            try {
                var propItem = image.GetPropertyItem((int)TagConstants.FocalLength);
                FocalLength = (Rational)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                FocalLength = 0.0;
            }
        }

        private void ExtractMake() {
            try {
                var propItem = image.GetPropertyItem((int)TagConstants.Make);
                Make = (string)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                Make = "";
            }
        }

        private void ExtractModel() {
            try {
                var propItem = image.GetPropertyItem((int)TagConstants.Model);
                Model = (string)PropertyTag.getValue(propItem);
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
                var propGpsLat = image.GetPropertyItem((int)TagConstants.Latitude);
                var propGpsLatRef = image.GetPropertyItem((int)TagConstants.GpsLatitudeRef);

                Rational[] latitudeDegMinSecs = (Rational[])PropertyTag.getValue(propGpsLat);
                string GpsLatRef = (string)PropertyTag.getValue(propGpsLatRef);

                Latitude = DegreesMinutesSecondsToDecimal(latitudeDegMinSecs, GpsLatRef);
            } catch (Exception) {
                // if there is no value set to default
                Latitude = 0;
            }
        }

        private void ExtractLongitude() {
            try {
                var propGpsLong = image.GetPropertyItem((int)TagConstants.Longitude);
                var propGpsLongRef = image.GetPropertyItem((int)TagConstants.GpsLongitudeRef);

                Rational[] longitudeDegMinSecs = (Rational[])PropertyTag.getValue(propGpsLong);
                string GpsLongRef = (string)PropertyTag.getValue(propGpsLongRef);

                Longitude = DegreesMinutesSecondsToDecimal(longitudeDegMinSecs, GpsLongRef);
            } catch (Exception) {
                // if there is no value set to default
                Longitude = 0;
            }
        }

        private double DegreesMinutesSecondsToDecimal(Rational[] degMinSec, string gpsRef) {
            if (degMinSec.Length != 3) return 0;

            double value = degMinSec[0] + (degMinSec[1] / 60d) + (degMinSec[2] / 3600d);
            
            // If Ref is not N or E, negate the value.
            if (gpsRef == "S" || gpsRef == "W") value *= -1;
            return value;
        }

        private void ExtractAltitude() {
            try {
                var propertyAltitude = image.GetPropertyItem((int)TagConstants.Altitude);
                var propertyAltitudeRef = image.GetPropertyItem((int)TagConstants.GpsAltitudeRef);
                var altRef = (int)PropertyTag.getValue(propertyAltitudeRef);

                Console.WriteLine(altRef);

                Altitude = (Rational)PropertyTag.getValue(propertyAltitude);
            } catch (Exception) {
                // if there is no value set to default
                Altitude = 0;
            }
        }

        public void ExtractMetadata() {
            ExtractDateTimeOriginal();
            ExtractISO();
            ExtractAperture();
            ExtractFNumber();
            ExtractShutterSpeed();
            ExtractFocalLength();
            ExtractMake();
            ExtractModel();
            ExtractResolution();
            ExtractLatitude();
            ExtractLongitude();
            ExtractAltitude();
        }

        public static int MD5HashFunction() {
            return 0; // TODO make function for getting MD5 hash of image
        }

        /// <summary> Converts the Exif DateTime format to yyyyMMdd_hhMMss </summary>
        private static string ExifDTToDateTime(string dtOrig) {
            dtOrig = dtOrig.Replace(' ', ':');
            string[] ymdHms = dtOrig.Split(':');

            string year    = ymdHms[0];
            string month   = ymdHms[1];
            string day     = ymdHms[2];
            string hour    = ymdHms[3];
            string minute  = ymdHms[4];
            string second  = ymdHms[5];
            
            return year + month + day + "_" + hour + minute + second;
        }
    }
}