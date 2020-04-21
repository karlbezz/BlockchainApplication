using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Extensions
{
    public static class BytesExtension
    {
        public static string ToStringValue(this byte[] valueBytes)
        {
            try
            {
                return Encoding.ASCII.GetString(valueBytes);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DateTime.Now} - Failed to convert bytes to string");
                throw e;
            }
        }

        public static int ToInt(this byte[] valueBytes)
        {
            try
            {
                return BitConverter.ToInt16(valueBytes, 0);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DateTime.Now} - Failed to convert bytes to int");
                throw e;
            }
        }

        public static long ToLong(this byte[] valueBytes)
        {
            try
            {
                return BitConverter.ToInt64(valueBytes, 0);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DateTime.Now} - Failed to convert bytes to long");
                throw e;
            }
        }

        public static byte[] GetRange(this byte[] valueBytes, int sourceStartIndex, int length, int destinationStartIndex = 0)
        {
            byte[] newValueBytes = new byte[length];
            Array.Copy(valueBytes, sourceStartIndex, newValueBytes, destinationStartIndex, length);
            return newValueBytes;
        }
    }
}
