using System;
using System.IO;
using System.Security.Cryptography;
using PhotoOrganizerLib.Enums;
using PhotoOrganizerLib.Interfaces;

namespace PhotoOrganizerLib.Utils
{
    /// <summary>
    /// Computes the file checksum using a specified hashing algorithm.
    /// </summary>
    public class Checksum : IChecksum, IDisposable
    {
        private readonly HashAlgorithm? _hashAlgorithm;
        /// <summary>
        /// Gets the name of the algorithm used for computing file checksum.
        /// </summary>
        public HashAlgorithmName AlgorithmName { get; private set; }

        /// <summary>
        /// Constructor for the Checksum class used for computing file checksum.
        /// </summary>
        /// <param name="algorithm">Algorithm to be used for hashing.</param>
        /// <remarks>
        /// Supported hash algorithms: { MD5, SHA1, SHA256 }.
        /// If <see langword="Algorithm.None" />, do not compute any file hash.
        /// Default hash algorithm is MD5, if input algorithm is unsupported or null.
        /// </remarks>
        public Checksum(Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Algorithm.None:
                    _hashAlgorithm = null;
                    AlgorithmName = new HashAlgorithmName("None");
                    break;
                case Algorithm.SHA1:
                    _hashAlgorithm = SHA1.Create();
                    AlgorithmName = HashAlgorithmName.SHA1;
                    break;
                case Algorithm.SHA256:
                    _hashAlgorithm = SHA256.Create();
                    AlgorithmName = HashAlgorithmName.SHA256;
                    break;
                case Algorithm.MD5:
                default:
                    _hashAlgorithm = MD5.Create();
                    AlgorithmName = HashAlgorithmName.MD5;
                    break;
            };
        }

        /// <summary>
        /// Computes checksum for a given <paramref name="stream" />.
        /// </summary>
        /// <returns>Checksum of input <paramref name="stream" />. Returns <see langword="null" /> if input or hashing algorithm is <see langword="null" />.</returns>
        /// <param name="stream">Stream to compute hash value.</param>
        public string? ComputeChecksum(Stream stream)
        {
            if (_hashAlgorithm is null || stream is null)
            {
                return null;
            }

            var hashValue = _hashAlgorithm.ComputeHash(stream);

            return BitConverter.ToString(hashValue)
                .Replace("-", string.Empty)
                .ToLowerInvariant();
        }

        /// <summary>
        /// Releases all resources used by current instance of the <see cref="Checksum" /> class.
        /// </summary>
        public void Dispose()
        {
            _hashAlgorithm?.Dispose();
        }
    }
}