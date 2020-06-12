using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.SBB.Protocol.Commands
{
    public class BlockHashesCommand : Command
    {
        [JsonProperty("hashes")]
        public List<string> Hashes { get; set; }

        public BlockHashesCommand()
        {
            CommandType = CommandType.BLOCK_HASHES;
        }
    }
}
