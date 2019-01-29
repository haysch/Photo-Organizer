using System;
using System.Drawing.Imaging;
using System.Text;

namespace PhotoOrganizer {
    public class PropertyTag {

        private static System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
        public static Object getValue (PropertyItem propItem) {
            if (propItem == null) return null;

            int size;
            byte[] value = propItem.Value;
            
            // if (BitConverter.IsLittleEndian)
            //     Array.Reverse(value);

            switch ((TagType)propItem.Type) {
                case TagType.Byte:
                    if (value.Length == 1) return value[0];
                    return value;
                
                case TagType.ASCII:
                    return encoder.GetString(value, 0, propItem.Len - 1);
                
                case TagType.Short:
                    size = 16 / 8;
                    ushort[] resUShort = new ushort[propItem.Len / size];
                    for (int i = 0; i < resUShort.Length; i++) {
                        resUShort[i] = BitConverter.ToUInt16(value, i * size);
                    }
                    if (resUShort.Length == 1) return resUShort[0];
                    return resUShort;
                
                case TagType.Long:
                    size = 32 / 8;
                    uint[] resUInt32 = new uint[propItem.Len / size];
                    for (int i = 0; i < resUInt32.Length; i++) {
                        resUInt32[i] = BitConverter.ToUInt32(value, i * size);
                    }
                    if (resUInt32.Length == 1) return resUInt32[0];
                    return resUInt32;
                
                case TagType.Rational:
                    size = 64 / 8;
                    Rational[] resRational = new Rational[propItem.Len / size];
                    long num;
                    long den;

                    for (int i = 0; i < resRational.Length; i++) {
                        num = BitConverter.ToUInt32(value, i * size);
                        den = BitConverter.ToUInt32(value, (i * size) + (size / 2));

                        resRational[i] = new Rational(num, den);
                    }
                    if (resRational.Length == 1) return resRational[0];
                    return resRational;
                
                case TagType.Undefined:
                    if (value.Length == 1) return value[0];
                    return value;
                
                case TagType.SLong:
                    size = 32 / 8;
                    int[] resInt32 = new int[propItem.Len / size];
                    for (int i = 0; i < resInt32.Length; i++) {
                        resInt32[i] = BitConverter.ToInt32(value, i * size);
                    }
                    if (resInt32.Length == 1) return resInt32[0];
                    return resInt32;
                
                case TagType.SRational:
                    size = 64 / 8;
                    Rational[] resSRational = new Rational[propItem.Len / size];
                    uint sNum;
                    uint sDen;

                    for (int i = 0; i < resSRational.Length; i++) {
                        sNum = BitConverter.ToUInt32(value, i * size);
                        sDen = BitConverter.ToUInt32(value, (i * size) + (size / 2));

                        resSRational[i] = new Rational(sNum, sDen);
                    }
                    if (resSRational.Length == 1) return resSRational[0];
                    return resSRational;
                
                default:
                    if (value.Length == 1) return value[0];
                    return value;
            }
        }
    }
}