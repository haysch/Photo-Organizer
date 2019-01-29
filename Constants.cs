namespace PhotoOrganizer {
    public enum TagConstants : int {
        /// <summary> ASCII </summary>
        DTOrig = 0x9003,
        /// <summary> Short </summary>
        ISO = 0x8827,
        /// <summary> Rational </summary>
        Aperture = 0x9202,
        /// <summary> Rational </summary>
        FNumber = 0x829D,
        /// <summary> SRational </summary>
        ShutterSpeed = 0x9201,
        /// <summary> Rational </summary>
        FocalLength = 0x920A,
        /// <summary> ASCII </summary>
        Make = 0x010F,
        /// <summary> ASCII </summary>
        Model = 0x0110,
        /// <summary> ASCII </summary>
        GpsLatitudeRef = 0x0001,
        /// <summary> Rational </summary>
        Latitude = 0x0002,
        /// <summary> ASCII </summary>
        GpsLongitudeRef = 0x0003,
        /// <summary> Rational </summary>
        Longitude = 0x0004,

        /// <summary> Byte </summary>
        GpsAltitudeRef = 0x0005,
        /// <summary> Rational </summary>
        Altitude = 0x0006
    };

    public enum TagType : short {
        /// <summary> Specifies that the format is 4 bits per pixel, indexed. </summary>
        PixelFormat4bppIndexed = 0,
        /// <summary> Specifies that the value data member is an array of bytes. </summary>
        Byte = 1,
        /// <summary> Specifies that the value data member is a null-terminated ASCII string. </summary>
        ASCII = 2,
        /// <summary> Specifies that the value data member is an array of unsigned short (16-bit) integers. </summary>
        Short = 3,
        /// <summary> Specifies that the value data member is an array of unsigned long (32-bit) integers. </summary>
        Long = 4,
        /// <summary> Specifies that the value data member is an array of pairs of unsigned long integers. Each pair represents a fraction; the first integer is the numerator, the second is the denominator. </summary>
        Rational = 5,
        /// <summary> Specifies that the value data member is an array of bytes that can hold values of any data type. </summary>
        Undefined = 7,
        /// <summary> Specifies that the value data member is an array of signed long (32-bit) integers. </summary>
        SLong = 9,
        /// <summary> Specifies that the value data member is an array of pairs of signed long integers. Each pair represents a fraction; the first integer is the numerator, the second is the denominator. </summary>
        SRational = 10
    };
}