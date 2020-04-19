using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Extensions
{
    public static class LongExtensions
    {
        public static byte[] ToBytes(this long valueLong)
        {
            try
            {
                return BitConverter.GetBytes(valueLong);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DateTime.Now} - Failed to convert long {valueLong} to bytes");
                throw e;
            }
        }
    }
}
