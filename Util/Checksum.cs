using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace PhotoOrganizer.Util
{
    /// <summary>Available hash algorithms</summary>
    public enum Algorithm : int
    {
        /// <summary>Message-Digest Algorithm 5</summary>
        /// <remarks>128 bit digest size</remarks>
        MD5,
        /// <summary>Secure Hash Algorithm 1</summary>
        /// <remarks>160 bit digest size</remarks>
        SHA1,
        /// <summary>Secure Hash Algorithm 2</summary>
        /// <remarks>256 bit digest size</remarks>
        SHA256,
        /// <summary>Secure Hash Algorithm 2</summary>
        /// <remarks>384 bit digest size</remarks>
        SHA384,
        /// <summary>Secure Hash Algorithm 2</summary>
        /// <remarks>512 bit digest size</remarks>
        SHA512
    };

    /// <summary>Specifies the Checksum class for use when computing hash values of files</summary>
    public class Checksum
    {
        private HashAlgorithm _cryptoAlgorithm;
        /// <summary>Constructor for the algorithm used for computing file hash.</summary>
        /// <exception>Throws an ArgumentException if input algorithm is not available.</exception>
        /// <param name="algorithm">Algorithm to be used.</param>
        /// See <see cref="Util.Algorithm"/> for available hash algorithms.
        public Checksum(Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Algorithm.MD5:
                    _cryptoAlgorithm = new MD5CryptoServiceProvider();
                    break;
                case Algorithm.SHA1:
                    _cryptoAlgorithm = new SHA1Managed();
                    break;
                case Algorithm.SHA256:
                    _cryptoAlgorithm = new SHA256Managed();
                    break;
                case Algorithm.SHA384:
                    _cryptoAlgorithm = new SHA384Managed();
                    break;
                case Algorithm.SHA512:
                    _cryptoAlgorithm = new SHA512Managed();
                    break;
                default:
                    throw new ArgumentException($"Invalid selection for {algorithm} algorithm.");
            }
        }

        /// <summary>Computes the hash value of the file.</summary>
        /// <returns>Hash value of file.</returns>
        /// <param name="path">String path.</param>
        public string ComputeHash(string path)
        {
            if (_cryptoAlgorithm == null || !File.Exists(path)) return "";

            using (FileStream stream = File.OpenRead(path))
            {
                byte[] hashBytes = _cryptoAlgorithm.ComputeHash(stream);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}