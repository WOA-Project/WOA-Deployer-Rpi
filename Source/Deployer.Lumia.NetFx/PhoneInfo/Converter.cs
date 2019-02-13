using System;
using System.Text;

namespace Deployer.Lumia.NetFx.PhoneInfo
{
    public static class Converter
    {
        public static string ConvertHexToString(byte[] Bytes, string Separator)
        {
            StringBuilder s = new StringBuilder(1000);
            for (int i = Bytes.GetLowerBound(0); i <= Bytes.GetUpperBound(0); i++)
            {
                if (i != Bytes.GetLowerBound(0))
                    s.Append(Separator);
                s.Append(Bytes[i].ToString("X2"));
            }
            return s.ToString();
        }

        public static byte[] ConvertStringToHex(string HexString)
        {
            if (HexString.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[HexString.Length >> 1];

            for (int i = 0; i < (HexString.Length >> 1); ++i)
            {
                arr[i] = (byte)((GetHexVal(HexString[i << 1]) << 4) + (GetHexVal(HexString[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

    }
}