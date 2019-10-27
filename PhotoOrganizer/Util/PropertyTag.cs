using System;
using System.Drawing.Imaging;
using System.Text;

using PhotoOrganizer.Primitives;

namespace PhotoOrganizer.Util
{
    /// <summary>Base class for property tag value conversion from byte[] to relevant value of the property tag type.</summary>
    public class PropertyTag
    {
        private static System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();

        /// <summary>Constructor for PropertyTag.</summary>
        public PropertyTag()
        {
        }

        /// <summary>Finds value of input property item.</summary>
        /// <returns>Object of the value of input propItem.</returns>
        /// <param name="propItem">A PropertyItem.</param>
        /// <remarks>Return object can be of multiple types and lengths. Can be null.</remarks>
        public static Object GetValue(PropertyItem propItem)
        {
            if (propItem == null) return null;

            int size;
            byte[] value = propItem.Value;

            switch ((PropertyTagType)propItem.Type)
            {
                case PropertyTagType.Byte:
                    if (value.Length == 1) return value[0];
                    return value;

                case PropertyTagType.ASCII:
                    return encoder.GetString(value, 0, propItem.Len - 1);

                case PropertyTagType.Short:
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(value);

                    size = 16 / 8;
                    ushort[] resultUShort = new ushort[propItem.Len / size];

                    for (int i = 0; i < resultUShort.Length; i++)
                    {
                        resultUShort[i] = BitConverter.ToUInt16(value, i * size);
                    }

                    if (resultUShort.Length == 1) return resultUShort[0];
                    return resultUShort;

                case PropertyTagType.Long:
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(value);
                    size = 32 / 8;
                    uint[] resultULong = new uint[propItem.Len / size];

                    for (int i = 0; i < resultULong.Length; i++)
                    {
                        resultULong[i] = BitConverter.ToUInt32(value, i * size);
                    }

                    if (resultULong.Length == 1) return resultULong[0];
                    return resultULong;

                case PropertyTagType.Rational:
                    size = 64 / 8;
                    Rational[] resultURational = new Rational[propItem.Len / size];
                    uint num;
                    uint den;

                    for (int i = 0; i < resultURational.Length; i++)
                    {
                        // Apparently this is needed to correctly compute byte[] to fraction correctly
                        // TODO figure out why
                        if (BitConverter.IsLittleEndian && (propItem.Id != (int)PropertyTagId.Latitude && propItem.Id != (int)PropertyTagId.Longitude))
                        {
                            Array.Reverse(value);
                            num = BitConverter.ToUInt32(value, (i * size) + (size / 2));
                            den = BitConverter.ToUInt32(value, i * size);
                        }
                        else
                        {
                            num = BitConverter.ToUInt32(value, i * size);
                            den = BitConverter.ToUInt32(value, (i * size) + (size / 2));
                        }

                        resultURational[i] = new Rational(num, den);
                    }

                    if (resultURational.Length == 1) return resultURational[0];
                    return resultURational;

                case PropertyTagType.Undefined:
                    if (value.Length == 1) return value[0];
                    return value;

                case PropertyTagType.SLong:
                    size = 32 / 8;
                    int[] resultSLong = new int[propItem.Len / size];

                    for (int i = 0; i < resultSLong.Length; i++)
                    {
                        resultSLong[i] = BitConverter.ToInt32(value, i * size);
                    }

                    if (resultSLong.Length == 1) return resultSLong[0];
                    return resultSLong;

                case PropertyTagType.SRational:
                    size = 64 / 8;
                    Rational[] resultSRational = new Rational[propItem.Len / size];
                    int sNum;
                    int sDen;

                    for (int i = 0; i < resultSRational.Length; i++)
                    {
                        sNum = BitConverter.ToInt32(value, i * size);
                        sDen = BitConverter.ToInt32(value, (i * size) + (size / 2));

                        resultSRational[i] = new Rational(sNum, sDen);
                    }

                    if (resultSRational.Length == 1) return resultSRational[0];
                    return resultSRational;

                default:
                    if (value.Length == 1) return value[0];
                    return value;
            }
        }
    }
}