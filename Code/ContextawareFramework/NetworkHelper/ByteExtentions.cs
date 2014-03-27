using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkHelper
{
    public static class ByteExtentions
    {
        public static byte[] GetBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string GetString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes.CropBytes());
        }

        public static string GetString(this ICollection<byte> bytes)
        {
            return Encoding.UTF8.GetString(bytes.ToArray());
        }


        private static byte[] CropBytes(this byte[] bytes)
        {
            var newArray = new byte[bytes.Count(t => t != 0)];

            for (var i = 0; i < newArray.Length; i++)
            {
                newArray[i] = bytes[i];
            }
            return newArray;
        }
    }
}