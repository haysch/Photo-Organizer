using System;
using System.IO;
using System.Collections.Generic;
using Xunit;

using PhotoOrganizer.Models;
using PhotoOrganizer.Util;
using PhotoOrganizerTest.Models;

using Newtonsoft.Json;

namespace PhotoOrganizerTest
{
    public class ExtractorTests
    {
        private Dictionary<string, object> _dict;
        private ExpectedImage _expectedImage;

        public ExtractorTests()
        {
            using (var reader = new StreamReader("TestFiles/testdata.json"))
            {
                var jsonString = reader.ReadToEnd();
                _expectedImage = JsonConvert.DeserializeObject<ExpectedImage>(jsonString);
            }

            var _extractor = new Extractor();
            var testImage = new ImageFile(_expectedImage.FileName, "TestFiles");
            _dict = _extractor.ExtractMetadata(testImage);
        }
        
        [Fact]
        public void ExtractHashValueTest()
        {
            var hashVal = _dict["HashValue"];
            var expected = _expectedImage.Expected.HashValue;

            Assert.Equal(hashVal, expected);
        }

        [Fact]
        public void ExtractDateTimeTest()
        {
            var dateTime = _dict["DateTime"];
            var datetimeString = _expectedImage.Expected.DateTime;
            var expected = DateTime.Parse(datetimeString);

            Assert.Equal(dateTime, expected);
        }

        [Fact]
        public void ExtractISOTest()
        {
            var iso = _dict["ISO"];
            var expected = _expectedImage.Expected.ISO;

            Assert.Equal(iso, expected);
        }
    }
}
