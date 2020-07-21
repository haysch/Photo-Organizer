using System;
using System.Globalization;
using System.Threading;
using MetadataExtractor;
using PhotoOrganizerLib.Utils;
using Xunit;

namespace PhotoOrganizerLib.Tests.Utils.Tests
{
    public class MetadataConverterTest
    {
        [Fact]
        public void ComputeGpsDmsToDecimalDegrees_NullDms()
        {
            var actual = MetadataConverter.DegreesMinutesSecondsToDecimalDegrees(null, "S");

            Assert.Null(actual);
        }
        
        [Fact]
        public void ComputeGpsDmsToDecimalDegrees_ValidDms_InvalidGpsReference()
        {
            var degMinSec = new Rational[3];

            degMinSec[0] = new Rational(1, 1);      // 1
            degMinSec[1] = new Rational(60, 1);     // 60
            degMinSec[2] = new Rational(3600, 1);   // 3600

            Assert.Throws<ArgumentException>(() => MetadataConverter.DegreesMinutesSecondsToDecimalDegrees(degMinSec, "A"));
        }

        [Fact]
        public void ComputeGpsDmsToDecimalDegrees_InvalidDms()
        {
            var degMinSec = new Rational[2];

            var actual = MetadataConverter.DegreesMinutesSecondsToDecimalDegrees(degMinSec, "");

            Assert.Null(actual);
        }

        [Fact]
        public void ComputeGpsDmsToDecimalDegrees_ValidDms_NorthGpsReference()
        {
            var degMinSec = new Rational[3];

            degMinSec[0] = new Rational(1, 1);      // 1
            degMinSec[1] = new Rational(60, 1);     // 60
            degMinSec[2] = new Rational(3600, 1);   // 3600

            var expected = 3.0; // 1 * (1 + 60/60 + 3600/3600) = 3.0

            var actual = MetadataConverter.DegreesMinutesSecondsToDecimalDegrees(degMinSec, "N");

            Assert.Equal(expected, actual);
        }
        
        
        [Fact]
        public void ComputeGpsDmsToDecimalDegrees_ValidDms_SouthGpsReference()
        {
            var degMinSec = new Rational[3];

            degMinSec[0] = new Rational(1, 1);      // 1
            degMinSec[1] = new Rational(60, 1);     // 60
            degMinSec[2] = new Rational(3600, 1);   // 3600

            var expected = -3.0; // -1 * (1 + 60/60 + 3600/3600) = -3.0

            var actual = MetadataConverter.DegreesMinutesSecondsToDecimalDegrees(degMinSec, "S");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ComputeShutterSpeed_LessOrEqualToOneValue()
        {
            // Align the data formatting for testing
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            var expected = "0.5 sec"; // round((1 / exp(1 * log(2))) * 10) / 10 = 0.5

            var actual = MetadataConverter.ComputeShutterSpeed(1);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ComputeShutterSpeed_GreaterThanOneValue()
        {
            var expected = "1/4 sec"; // exp(2 * log(2)) = 4
            
            var actual = MetadataConverter.ComputeShutterSpeed(2);
            
            Assert.Equal(expected, actual);
        }
    }
}