using BlockchainApplication.Data.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainApplication.SBB
{
    public class SBBState
    {
        public Wallet Wallet { get; set; }
        public List<Block> Blocks { get; set; }
        public SBBNodeState NodeState { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }

        public List<string> Logger { get; set; }

        public SBBState()
        {
            this.Blocks = new List<Block>();
            this.Wallet = new Wallet();
            this.NodeState = SBBNodeState.AVAILABLE;
            this.Logger = new List<string>();
        }

        public SBBState(string ip, string port)
        {
            this.IP = ip;
            this.Port = int.Parse(port);
            this.Blocks = new List<Block>();
            this.Wallet = new Wallet();
            this.NodeState = SBBNodeState.AVAILABLE;
            this.Logger = new List<string>();
        }

    }
}
