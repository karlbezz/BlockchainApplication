using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.SBB.Protocol.Commands
{
    public class CountCommand : Command
    {
        [JsonProperty("blocks")]
        public int Blocks { get; set; }

        public CountCommand()
        {
            CommandType = CommandType.COUNT;
        }
    }
}
