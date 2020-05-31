using BlockchainApplication.Data.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace BlockchainApplication.SBB
{
    public class SBBState
    {
        public Wallet Wallet { get; set; }
        public List<Block> Blocks { get; set; }
        public SBBNodeState NodeState { get; set; }

        public SBBState()
        {
            Blocks = new List<Block>();
            Wallet = new Wallet();
            NodeState = SBBNodeState.AVAILABLE;
        }
    }
}
