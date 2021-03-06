using PhotoOrganizerLib.Extensions;
using System.IO;
using Xunit;

namespace PhotoOrganizerLib.Tests.Extensions.Tests
{
    public class StringExtensionsTest
    {
        [Fact]
        public void IsYear_True()
        {
            var yearString = "2000";

            var actual = yearString.IsYear();

            Assert.True(actual);
        }

        [Fact]
        public void IsYear_False()
        {
            var yearString = "25b";

            var actual = yearString.IsYear();

            Assert.False(actual);
        }

        [Fact]
        public void IsYear_Null()
        {
            string yearString = null;

            var actual = yearString.IsYear();

            Assert.False(actual);
        }

        [Fact]
        public void IsMonth_True()
        {
            var monthString = "05";

            var actual = monthString.IsMonth();

            Assert.True(actual);
        }

        [Fact]
        public void IsMonth_False()
        {
            var monthString = "13";

            var actual = monthString.IsMonth();

            Assert.False(actual);
        }

        [Fact]
        public void IsMonth_Null()
        {
            string monthString = null;

            var actual = monthString.IsMonth();

            Assert.False(actual);
        }

        [Fact]
        public void EnsureDirectoryExists_Empty_NoMessage()
        {
            var path = string.Empty;

            var message = Assert.Throws<DirectoryNotFoundException>(() => path.EnsureDirectoryExists()).Message;

            Assert.NotEmpty(message);
        }

        [Fact]
        public void EnsureDirectoryExists_Empty_WithMessage()
        {
            var path = string.Empty;
            var expectedMessage = "Path is empty.";

            var message = Assert.Throws<DirectoryNotFoundException>(() => path.EnsureDirectoryExists(expectedMessage)).Message;

            Assert.Equal(expectedMessage, message);
        }

        [Fact]
        public void EnsureDirectoryExists_ValidPath_NoMessage()
        {
            var path = Path.GetTempPath();

            path.EnsureDirectoryExists();
        }
    }
}