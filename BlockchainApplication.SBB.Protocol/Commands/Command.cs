using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.SBB.Protocol.Commands
{
    public class Command
    {
        [JsonIgnore]
        public CommandType CommandType { get; set; }
    }
}
