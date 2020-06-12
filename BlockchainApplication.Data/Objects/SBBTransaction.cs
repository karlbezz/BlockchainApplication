using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Objects
{
    public class SBBTransaction
    {
        [JsonProperty("hashedContent")]
        public TransactionHashedContent HashedContent { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        public SBBTransaction()
        {
            this.HashedContent = new TransactionHashedContent();
        }

        public SBBTransaction(string from, string to)
        {
            this.HashedContent = new TransactionHashedContent()
            {
                FromAccount = from,
                ToAccount = to
            };
        }
    }
}
