using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using PhotoOrganizerLib.Enums;
using PhotoOrganizerLib.Utils;
using Xunit;

namespace PhotoOrganizerTest.Utils.Tests
{
    public class ChecksumTests
    {
        [Fact]
        public void ChecksumConstructor_SupportedAlgorithms()
        {
            var algorithmList = new List<string> { "None", "MD5", "SHA1", "SHA256" };

            foreach (var alg in algorithmList)
            {
                var algorithm = Enum.Parse<Algorithm>(alg);
                using var cs = new Checksum(algorithm);

                Assert.Equal(alg, cs.AlgorithmName.Name);
            }
        }

        [Fact]
        public void ChecksumConstructor_NullAlgorithm()
        {
            Enum.TryParse("null", out Algorithm test);
            using var cs = new Checksum(test);

            Assert.Equal("MD5", cs.AlgorithmName.Name);
        }

        [Fact]
        public void Checksum_ComputeChecksum_MD5()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, World!"));
            using var cs = new Checksum(Algorithm.MD5);

            // Compute checksum, consuming stream
            var actual = cs.ComputeChecksum(stream);

            // Reset stream
            stream.Position = 0;

            // Compute checksum using expected behavior
            using var md5 = MD5.Create();
            var md5hash = md5.ComputeHash(stream);
            var expected = BitConverter.ToString(md5hash)
                .Replace("-", string.Empty)
                .ToLowerInvariant();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Checksum_ComputeChecksum_SHA1()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, World!"));
            using var cs = new Checksum(Algorithm.SHA1);

            // Compute checksum, consuming stream
            var actual = cs.ComputeChecksum(stream);

            // Reset stream
            stream.Position = 0;

            // Compute checksum using expected behavior
            using var sha1 = SHA1.Create();
            var sha1hash = sha1.ComputeHash(stream);
            var expected = BitConverter.ToString(sha1hash)
                .Replace("-", string.Empty)
                .ToLowerInvariant();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Checksum_ComputeChecksum_SHA256()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, World!"));
            using var cs = new Checksum(Algorithm.SHA256);

            // Compute checksum, consuming stream
            var actual = cs.ComputeChecksum(stream);

            // Reset stream
            stream.Position = 0;

            // Compute checksum using expected behavior
            using var sha256 = SHA256.Create();
            var sha256hash = sha256.ComputeHash(stream);
            var expected = BitConverter.ToString(sha256hash)
                .Replace("-", string.Empty)
                .ToLowerInvariant();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Checksum_ComputeChecksum_NullStream()
        {
            using var cs = new Checksum(Algorithm.MD5);

            var actual = cs.ComputeChecksum(null);

            Assert.Empty(actual);
        }

        [Fact]
        public void Checksum_ComputeChecksum_NoneAlgorithm()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, World!"));
            var cs = new Checksum(Algorithm.None);

            var actual = cs.ComputeChecksum(stream);

            Assert.Empty(actual);
        }
    }
}