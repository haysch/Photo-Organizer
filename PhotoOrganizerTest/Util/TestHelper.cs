using System.IO;
using Newtonsoft.Json;
using PhotoOrganizerTest.Models;

namespace PhotoOrganizerTest.Util {
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