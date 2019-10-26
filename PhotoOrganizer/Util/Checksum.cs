using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

using PhotoOrganizer.Primitives;

namespace PhotoOrganizer.Util
{

    /// <summary>Specifies the Checksum class for use when computing hash values of files</summary>
    public class Checksum
    {
        private HashAlgorithm _cryptoAlgorithm;
        /// <summary>Gets the algorithm used for hashing.</summary>
        public string HashAlg { get; }

        /// <summary>Constructor for the algorithm used for computing file hash.</summary>
        /// <exception cref="System.ArgumentException">Throws an ArgumentException if input algorithm is not supported.</exception>
        /// <param name="algorithm">Algorithm to be used.</param>
        /// See <see cref="Primitives.Algorithm" /> for available hash algorithms.
        public Checksum(Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Algorithm.None:
                    _cryptoAlgorithm = null;
                    HashAlg = "None";
                    break;
                case Algorithm.MD5:
                    _cryptoAlgorithm = new MD5CryptoServiceProvider();
                    HashAlg = "MD5";
                    break;
                case Algorithm.SHA1:
                    _cryptoAlgorithm = new SHA1Managed();
                    HashAlg = "SHA1";
                    break;
                case Algorithm.SHA256:
                    _cryptoAlgorithm = new SHA256Managed();
                    HashAlg = "SHA256";
                    break;
                case Algorithm.SHA384:
                    _cryptoAlgorithm = new SHA384Managed();
                    HashAlg = "SHA384";
                    break;
                case Algorithm.SHA512:
                    _cryptoAlgorithm = new SHA512Managed();
                    HashAlg = "SHA512";
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
            if (_cryptoAlgorithm == null || !File.Exists(path))
                return "";

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