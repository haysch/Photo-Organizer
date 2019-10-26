using System;
using Xunit;
using PhotoOrganizer.Models;
using PhotoOrganizer.Util;
using System.Collections.Generic;

namespace PhotoOrganizerTest
{
    public class ExtractorTests
    {
        private Dictionary<string, object> _dict;
        public ExtractorTests()
        {
            var _extractor = new Extractor();
            var testImage = new ImageFile("temple.jpg", "testdata");
            _dict = _extractor.ExtractMetadata(testImage);
        }
        
        [Fact]
        public void ExtractHashValueTest()
        {
            var hashVal = _dict["HashValue"];
            var expected = "6556a73d7537fa6323e9ca429c66658c";

            Assert.Equal(hashVal, expected);
        }

        [Fact]
        public void ExtractDateTimeTest()
        {
            var dateTime = _dict["DateTime"];
            var expected = new DateTime(2018, 7, 10, 15, 18, 40);

            Assert.Equal(dateTime, expected);
        }

        [Fact]
        public void ExtractISOTest()
        {
            var iso = _dict["ISO"];
            ushort expected = 20;

            Assert.Equal(iso, expected);
        }
    }
}
