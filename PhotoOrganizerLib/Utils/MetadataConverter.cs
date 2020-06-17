using System;
using MetadataExtractor;

namespace PhotoOrganizerLib.Utils
{
    public static class MetadataConverter
    {
        /// <summary>Converts Degree/Minutes/Seconds to decimal degrees.</summary>
        /// <returns>Double of the coordinate.</returns>
        /// <remarks>If input array is not of size 3, return 0.0.</remarks>
        /// <param name="degMinSec">Rational array containing the Degree/Minutes/Seconds.</param>
        /// <param name="gpsRef">String containing GPS reference direction, e.g. "N" or "E".</param>
        public static double DegreesMinutesSecondsToDecimal(Rational[] degMinSec, string gpsRef)
        {
            if (degMinSec?.Length != 3 || gpsRef is null) 
                return 0;

            var hours = Math.Abs(degMinSec[0].ToDouble());
            double minutes = degMinSec[1].ToDouble();
            double seconds = degMinSec[2].ToDouble();

            double value = hours + (minutes / 60.0d) + (seconds / 3600.0d);

            // If Ref is not N or E, negate the value.
            if (gpsRef == "S" || gpsRef == "W")
                value *= -1;
            
            return value;
        }

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