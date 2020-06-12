using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Objects
{
    public class TransactionHashedContent
    {
        [JsonProperty("from_ac")]
        public string FromAccount { get; set; }

        [JsonProperty("to_ac")]
        public string ToAccount { get; set; }
    }
}
