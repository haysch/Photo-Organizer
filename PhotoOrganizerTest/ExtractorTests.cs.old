using System;
using System.Collections.Generic;
using Xunit;

using PhotoOrganizerLib.Models;

using PhotoOrganizerTest.Models;
using PhotoOrganizerTest.Util;

namespace PhotoOrganizerTest
{
    public class ExtractorTests
    {
        private Dictionary<string, object> _dict;
        private TestData _testData;

        public ExtractorTests()
        {
            _testData = TestHelper.LoadTestData("TestFiles/testdata.json");

            var testImage = new Photo(_testData.FileName, "TestFiles");
            // _dict = ExtractMetadata(testImage);
        }

        [Fact]
        public void ExtractHashValueTest()
        {
            var hashVal = _dict["HashValue"];
            var expected = _testData.Expected.HashValue;

            Assert.Equal(hashVal, expected);
        }

        [Fact]
        public void ExtractDateTimeTest()
        {
            var dateTime = _dict["DateTime"];
            var datetimeString = _testData.Expected.DateTime;
            var expected = DateTime.Parse(datetimeString);

            Assert.Equal(dateTime, expected);
        }

        [Fact]
        public void ExtractISOTest()
        {
            var iso = _dict["ISO"];
            var expected = _testData.Expected.ISO;

            Assert.Equal(iso, expected);
        }
    }
}
