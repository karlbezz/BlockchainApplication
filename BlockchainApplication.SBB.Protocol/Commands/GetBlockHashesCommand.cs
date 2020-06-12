using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.SBB.Protocol.Commands
{
    public class GetBlockHashesCommand : Command
    {
        public GetBlockHashesCommand()
        {
            CommandType = CommandType.GET_BLOCK_HASHES;
        }
    }
}
