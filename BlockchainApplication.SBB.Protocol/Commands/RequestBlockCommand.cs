using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.SBB.Protocol.Commands
{
    public class RequestBlockCommand : Command
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        public RequestBlockCommand()
        {
            CommandType = CommandType.REQ_BLOCK;
        }

        public RequestBlockCommand(string hash)
        {
            CommandType = CommandType.REQ_BLOCK;
            Hash = hash;
        }
    }
}
