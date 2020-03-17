using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

using PhotoOrganizerLib.Enums;
using PhotoOrganizerLib.Interfaces;

namespace PhotoOrganizerLib.Utils
{

    /// <summary>Specifies the Checksum class for use when computing hash values of files</summary>
    public class Checksum : IChecksum
    {
        private HashAlgorithm _cryptoAlgorithm;
        /// <summary>Gets the algorithm used for computing the checksum.</summary>
        public string HashAlgorithm 
        { 
            get => _cryptoAlgorithm.ToString();
        }

        /// <summary>Constructor for the Checksum class used for computing file hash.</summary>
        /// <exception cref="System.ArgumentException">Throws an ArgumentException if input algorithm is not supported.</exception>
        /// <param name="algorithm">Algorithm to be used.</param>
        /// <remarks>See <see cref="PhotoOrganizerLib.Enums.Algorithm" /> for available hash algorithms.</remarks>
        public Checksum(Algorithm algorithm)
        {
            _cryptoAlgorithm = (algorithm) switch
            {
                Algorithm.None      => null,
                Algorithm.MD5       => new MD5CryptoServiceProvider(),
                Algorithm.SHA1      => new SHA1Managed(),
                Algorithm.SHA256    => new SHA256Managed(),
                Algorithm.SHA384    => new SHA384Managed(),
                Algorithm.SHA512    => new SHA512Managed(),
                _                   => throw new ArgumentException($"Invalid selection for {algorithm} algorithm.")
            };
        }

        /// <summary>Computes the hash checksum of the file.</summary>
        /// <returns>Checksum of file.</returns>
        /// <param name="filePath">String path to the file.</param>
        public string ComputeChecksum(string filePath)
        {
            if (_cryptoAlgorithm == null || !File.Exists(filePath))
                return string.Empty;

            using (var stream = File.OpenRead(filePath))
            {
                var hashBytes = _cryptoAlgorithm.ComputeHash(stream);

                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}