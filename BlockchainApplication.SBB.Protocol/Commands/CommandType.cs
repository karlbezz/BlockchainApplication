using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.SBB.Protocol.Commands
{
    public enum CommandType
    {
        GET_COUNT = 'a',
        COUNT = 'c',
        GET_BLOCK_HASHES = 'b',
        BLOCK_HASHES = 'h',
        REQ_BLOCK = 'r',
        BLOCK = 'x',
        NEW_BLOCK = 'z'
    }
}
