using BlockchainApplication.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Utilities
{
    public static class DateTimeUtilities
    {
        public static long ConvertToUnixTimestamp(DateTime currentDate)
        {
            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            long unixTime = ((DateTime.Now.Ticks - epochTicks) / TimeSpan.TicksPerSecond);
            return unixTime;
        }

        public static byte[] ConvertToUnixTimestampBytes(DateTime currentDate)
        {
            return ConvertToUnixTimestamp(currentDate).ToBytes();
        }
    }
}
