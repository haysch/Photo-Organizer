namespace PhotoOrganizerLib.Enums
{
    /// <summary>Available hash algorithms</summary>
    public enum Algorithm
    {
        /// <summary>No hash value will be computed.</summary>
        None = 0,
        /// <summary>Message-Digest Algorithm 5.</summary>
        /// <remarks>128 bit digest size.</remarks>
        MD5 = 1,
        /// <summary>Secure Hash Algorithm 1.</summary>
        /// <remarks>160 bit digest size.</remarks>
        SHA1 = 2,
        /// <summary>Secure Hash Algorithm 2.</summary>
        /// <remarks>256 bit digest size.</remarks>
        SHA256 = 3,
        /// <summary>Secure Hash Algorithm 2.</summary>
        /// <remarks>384 bit digest size.</remarks>
        SHA384 = 4,
        /// <summary>Secure Hash Algorithm 2.</summary>
        /// <remarks>512 bit digest size.</remarks>
        SHA512 = 5
    };
}