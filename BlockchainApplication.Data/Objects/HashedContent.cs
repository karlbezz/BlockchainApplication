using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Objects
{
    public class HashedContent
    {
        [JsonProperty("prev_hash")]
        public string PreviousBlockHash { get; set; }

        [JsonProperty("nonce")]
        public int Nonce { get; set; }

        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }

        [JsonProperty("transactions")]
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
