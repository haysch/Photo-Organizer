using System.IO;
using Newtonsoft.Json;
using PhotoOrganizerLib.Tests.Models;

namespace PhotoOrganizerLib.Tests.Util {
    public static class TestHelper {
        public static TestData LoadTestData(string path) {
            TestData testData;

            using (var reader = new StreamReader(path))
            {
                var jsonString = reader.ReadToEnd();
                testData = JsonConvert.DeserializeObject<TestData>(jsonString);
            }

            return testData;
        }
    }
}