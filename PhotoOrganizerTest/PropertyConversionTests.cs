using Xunit;
using System;
using System.Linq;

using PhotoOrganizer.Models;
using PhotoOrganizer.Primitives;
using PhotoOrganizer.Util;

using PhotoOrganizerTest.Models;
using PhotoOrganizerTest.Util;

namespace PhotoOrganizerTest
{
    // Test of iPhone images
    public class PropertyConversionTests
    {
        private ImageFile _imageFile;
        private TestData _testData;

        public PropertyConversionTests()
        {
            _testData = TestHelper.LoadTestData("TestFiles/testdata.json");
            _imageFile = new ImageFile(_testData.FileName, "TestFiles");
        }

        [Fact]
        public void AsciiDateTimePropertyTest()
        {
            int dateTimeProbId = (int) PropertyTagId.DateTime;
            var dateTimeProperty = _imageFile.ImageData.GetPropertyItem(dateTimeProbId);

            string result = (string) PropertyTag.GetValue(dateTimeProperty);
            string expected = "2018:07:10 15:18:40";

            Assert.Equal(result, expected);
        }

        [Fact]
        public void AsciiGpsLatitudeRefTest()
        {
            int gpsLatRefProbId = (int) PropertyTagId.GpsLatitudeRef;
            var gpsLatRefProperty = _imageFile.ImageData.GetPropertyItem(gpsLatRefProbId);

            string result = (string) PropertyTag.GetValue(gpsLatRefProperty);
            string expected = "N";

            Assert.Equal(result, expected);
        }

        [Fact]
        public void AsciiGpsLongitudeRefTest()
        {
            int gpsLongRefProbId = (int) PropertyTagId.GpsLongitudeRef;
            var gpsLongRefProperty = _imageFile.ImageData.GetPropertyItem(gpsLongRefProbId);

            string result = (string) PropertyTag.GetValue(gpsLongRefProperty);
            string expected = "E";

            Assert.Equal(result, expected);
        }

        [Fact]
        public void ShortISOPropertyTest()
        {
            int isoPropId = (int) PropertyTagId.ISO;
            var isoProperty = _imageFile.ImageData.GetPropertyItem(isoPropId);

            ushort result = (ushort) PropertyTag.GetValue(isoProperty);
            ushort expected = 20;

            Assert.Equal(result, expected);
        }
    }
}