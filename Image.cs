using System;
using System.Text;
using System.Drawing;
using System.IO;

namespace PhotoOrganizer {
    class Image {
        public string DateTimeOriginal { get; internal set; }
        public ushort ISO { get; internal set; }   
        public Rational Aperture { get; internal set; }
        // TODO Fnumber
        public Rational ShutterSpeed { get; internal set; }
        public Rational FocalLength { get; internal set; }
        public string Make { get; internal set; }
        public string Model { get; internal set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public double Longitude { get; internal set; }
        public double Latitude { get; internal set; }
        public double Altitude { get; internal set; }

        private Bitmap img;

        // TODO add hashmap when initializing to get all propItems and call when needed ?
        public Image(string imgName, string path) {
            // TODO test deeper enumeration of path (subfolders)
            img = new Bitmap(path + Path.DirectorySeparatorChar + imgName);
            
            // var propItems = img.PropertyItems;

            // foreach (var item in propItems) {
            //     Console.WriteLine(item.Id);
            // }
        }

        /// <summary> Extracts the DateTimeOriginal property from image and converts it to ASCII string. </summary>
        /// <remarks> If the prop does not exist, assign variable to default value. </remarks>
        public void extractDateTimeOriginal() {
            try {
                var propItem = img.GetPropertyItem((int)TagConstants.DateTime);
                DateTimeOriginal = (string)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                DateTimeOriginal = "";
            }
        }

        /// <summary> Extracts the ISO property from image and converts it to short string. </summary>
        /// <remarks> If the prop does not exist, assign variable to default value. </remarks>
        public void extractISO() {
            try {
                var propItem = img.GetPropertyItem((int)TagConstants.ISO);
                ISO = (ushort)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                ISO = 0;
            }
        }

        public void extractAperture() {
            try {
                var propItem = img.GetPropertyItem((int)TagConstants.Aperture);
                Aperture = (Rational)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                Aperture = new Rational(0);
            }
        }

        public void extractShutterSpeed() {
            try {
                var propItem = img.GetPropertyItem((int)TagConstants.ShutterSpeed);
                ShutterSpeed = (Rational)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                ShutterSpeed = new Rational(0);
            }
        }

        public void extractFocalLength() {
            try {
                var propItem = img.GetPropertyItem((int)TagConstants.FocalLength);
                FocalLength = (Rational)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                FocalLength = new Rational(0);
            }
        }

        public void extractMake() {
            try {
                var propItem = img.GetPropertyItem((int)TagConstants.Make);
                Make = (string)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                Make = "";
            }
        }

        public void extractModel() {
            try {
                var propItem = img.GetPropertyItem((int)TagConstants.Model);
                Model = (string)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                Model = "";
            }
        }

        // TODO does not get resolution - something wrong with width/height or multiplier
        public void GetResolution() {
            try {
                var propWidth = img.GetPropertyItem((int)TagConstants.XResolution);
                var resWidth = (Rational)PropertyTag.getValue(propWidth);
                var propHeight = img.GetPropertyItem((int)TagConstants.YResolution);
                var resHeight = (Rational)PropertyTag.getValue(propHeight);

                Console.WriteLine("Width: {0}\nHeight: {1}", resWidth, resHeight);

            } catch (Exception) {

            }
        }
        public void extractWidth() {
            try {
                var propItem = img.GetPropertyItem((int)TagConstants.XResolution);
                Width = (int)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                Width = 0;
            }
        }

        public void extractHeight() {
            try {
                var propItem = img.GetPropertyItem((int)TagConstants.YResolution);
                Height = (int)PropertyTag.getValue(propItem);
            } catch (Exception) {
                // if there is no value set to default
                Height = 0;
            }
        }

        /// <summary> Gets the latitude from the image in decimal format </summary>
        public void extractLatitude() {
            try {
                var propGpsLat = img.GetPropertyItem((int)TagConstants.Latitude);
                var propGpsLatRef = img.GetPropertyItem((int)TagConstants.GpsLatitudeRef);

                Rational[] latitudeDegMinSecs = (Rational[])PropertyTag.getValue(propGpsLat);
                string GpsLatRef = (string)PropertyTag.getValue(propGpsLatRef);

                Latitude = DegreesMinutesSecondsToDecimal(latitudeDegMinSecs, GpsLatRef);
            } catch (Exception) {
                // if there is no value set to default
                Latitude = 0;
            }
        }

        public void extractLongitude() {
            try {
                var propGpsLong = img.GetPropertyItem((int)TagConstants.Longitude);
                var propGpsLongRef = img.GetPropertyItem((int)TagConstants.GpsLongitudeRef);

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
            
            if (gpsRef == "S" || gpsRef == "W") value *= -1;
            return value;
        }

        public void extractAltitude() {
            string description;
            
            try {
                var propItem = img.GetPropertyItem((int)TagConstants.Altitude);
                Altitude = (Rational)PropertyTag.getValue(propItem);
            } catch (Exception e) {
                // if there is no value set to default
                description = $"EXCEPTION: {e.Message}";
                Altitude = 0;
            }
        }
    }
}