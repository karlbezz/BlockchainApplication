using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Extensions
{
    public static class IntExtensions
    {
        public static byte ToByte(this int valueInt)
        {
            try
            {   
                return Convert.ToByte(valueInt);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DateTime.Now} - Failed to convert int {valueInt} to byte");
                throw e;
            }
        }

        public static byte[] ToBytesBigEndian(this int valueInt)
        {
            try
            {
                byte[] numberBytes = BitConverter.GetBytes((Int16)valueInt).Reverse().ToArray();
                return numberBytes;
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DateTime.Now} - Failed to convert int {valueInt} to bytes");
                throw e;
            }
        }

        public static byte[] ToBytes(this int valueInt)
        {
            try
            {
                byte[] numberBytes = BitConverter.GetBytes((Int16)valueInt);
                return numberBytes;
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DateTime.Now} - Failed to convert int {valueInt} to bytes");
                throw e;
            }
        }

        public static byte[] ToBytesInt32(this int valueInt)
        {
            try
            {
                byte[] numberBytes = BitConverter.GetBytes((Int32)valueInt).Reverse().ToArray();
                return numberBytes;
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DateTime.Now} - Failed to convert int {valueInt} to bytes");
                throw e;
            }
        }
    }
}
