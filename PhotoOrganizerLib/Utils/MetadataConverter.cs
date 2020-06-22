using System;
using MetadataExtractor;

namespace PhotoOrganizerLib.Utils
{
    public static class MetadataConverter
    {
        /// <summary>Converts degrees/minutes/seconds to decimal degrees.</summary>
        /// <returns>Double of the decimal degrees.</returns>
        /// <remarks>If dms is not of size 3 or gpsRef is null, return null.</remarks>
        /// <param name="dms">Rational array containing the degrees/minutes/seconds.</param>
        /// <param name="gpsRef">String containing GPS reference direction, e.g. "N" or "E".</param>
        /// <exception cref="System.ArgumentException">Thrown when gpsRef is not "N", "E", "W", "S".</exception>
        public static double? DegreesMinutesSecondsToDecimalDegrees(Rational[] dms, string gpsRef)
        {
            if (dms?.Length != 3 || gpsRef is null)
            {
                return null;
            }

            var refMultiplier = gpsRef.ToUpper() switch {
                string s when s == "S" || s == "W" => -1,
                string s when s == "E" || s == "N" => 1,
                _ => throw new ArgumentException("GPS Reference direction is invalid.")
            };

            var degrees = dms[0].ToDouble();
            var minutes = dms[1].ToDouble();
            var seconds = dms[2].ToDouble();

            return (degrees + (minutes / 60.0d) + (seconds / 3600.0d)) * refMultiplier;
        }

        /// <summary>Convert apex value from MetadataExtractor to shutter speed format.</summary>
        /// <remarks>Copied from https://github.com/drewnoakes/metadata-extractor-dotnet/blob/master/MetadataExtractor/TagDescriptor.cs method GetShutterSpeedDescription</remarks>
        public static string ComputeShutterSpeed(float apexValue)
        {
            if (apexValue <= 1)
            {
                var apexPower = (float)(1 / Math.Exp(apexValue * Math.Log(2)));
                var apexPower10 = (long)Math.Round(apexPower * 10.0);
                var fApexPower = apexPower10 / 10.0f;
                return fApexPower + " sec";
            }
            else
            {
                var apexPower = (int)Math.Exp(apexValue * Math.Log(2));
                return "1/" + apexPower + " sec";
            }
        }
    }
}