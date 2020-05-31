using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Objects
{
    public class HashedContent
    {
        public string PreviousBlockHash { get; set; }
        public int Nonce { get; set; }
        public int Timestamp { get; set; }
        public List<SBBTransaction> Transactions { get; set; }

        public HashedContent()
        {
            this.Transactions = new List<SBBTransaction>();
        }
        public HashedContent(string previousBlockHash, int timestamp, List<SBBTransaction> transactions)
        {
            this.PreviousBlockHash = previousBlockHash;
            this.Timestamp = timestamp;
            this.Transactions = new List<SBBTransaction>(transactions);
        }

        public HashedContent(string previousBlockHash, int timestamp)
        {
            this.PreviousBlockHash = previousBlockHash;
            this.Timestamp = timestamp;
            this.Transactions = new List<SBBTransaction>();
        }
    }
}
