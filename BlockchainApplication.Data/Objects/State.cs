using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Objects
{
    public class State
    {
        public string Username { get; set; }
        public List<Transaction> Transactions { get; set; }
        public Dictionary<string, int> Balances { get; set; }
        public bool Syncing { get; set; }
    }
}
