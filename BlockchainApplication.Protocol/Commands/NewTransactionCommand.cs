using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Protocol.Commands
{
    public class NewTransactionCommand : Command
    {
        public int TransactionNumber { get; set; }
        public long Timestamp { get; set; }
        public string FromUser { get; set; }
        public string ToUser { get; set; }
    }
}
