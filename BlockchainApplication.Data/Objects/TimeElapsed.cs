using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Objects
{
    public class TimeElapsed
    {
        public DateTime CurrentTime { get; set; }
        public int Tries { get; set; }

        public TimeElapsed()
        {
            CurrentTime = DateTime.Now;
            Tries = 0;
        }
    }
}
