using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Objects
{
    public class SBBTransaction
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Hash { get; set; }

        public SBBTransaction(string from, string to)
        {
            this.From = from;
            this.To = to;
        }
    }
}
