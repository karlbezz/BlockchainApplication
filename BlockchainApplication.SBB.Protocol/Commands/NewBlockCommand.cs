using BlockchainApplication.Data.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.SBB.Protocol.Commands
{
    public class NewBlockCommand : Command
    {
        [JsonProperty("block")]
        public Block Block { get; set; }

        public NewBlockCommand()
        {
            CommandType = CommandType.NEW_BLOCK;
        }
    }
}
