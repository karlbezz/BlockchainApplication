using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Extensions
{
    public static class StringExtensions
    {
        public static int ToInteger(this string valueString)
        {
            try
            {
                return int.Parse(valueString);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DateTime.Now} - Failed to convert string {valueString} to integer");
                throw e;
            }   
        }

        public static byte[] ToBytes(this string valueString)
        {
            try
            {
                return Encoding.ASCII.GetBytes(valueString);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DateTime.Now} - Failed to convert string {valueString} to bytes");
                throw e;
            }
        }
    }
}
