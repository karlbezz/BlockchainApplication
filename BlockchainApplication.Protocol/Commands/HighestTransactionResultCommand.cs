using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Protocol.Commands
{
    public class HighestTransactionResultCommand : Command
    {
        public int TransactionNumber { get; set; }
    }
}
