using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.SBB.Protocol.Commands
{
    public class GetCountCommand : Command
    {
        public GetCountCommand()
        {
            this.CommandType = CommandType.GET_COUNT;
        }
    }
}
