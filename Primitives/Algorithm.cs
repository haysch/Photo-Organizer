namespace PhotoOrganizer.Util
{
    /// <summary>Available hash algorithms</summary>
    public enum Algorithm
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
}