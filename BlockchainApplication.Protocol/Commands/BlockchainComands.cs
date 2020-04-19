using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Protocol.Commands
{
    public enum BlockchainCommands
    {
        ANY,
        NEW_TRANS,
        HIGHEST_TRN,
        HIGHEST_TRN_RES,
        GET_TRANS,
        OK_MSG,
        NOK_MSG,
    }
}
