using BlockchainApplication.Data.Objects;
using BlockchainApplication.Data.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainApplication.SBB
{
    public static class Utilities
    {
        public static bool ConfirmBlock(Block block, string hash = "")
        {
            string data = JsonConvert.SerializeObject(block.HashedContent);
            string blockHash = Hashing.GenerateHash(data);
            return hash == "" ? blockHash == block.Hash : hash == blockHash;
        }

        public static void Delay(SBBState state)
        {
            while (state.NodeState != SBBNodeState.AVAILABLE)
            {
                Thread.Sleep(1000);
            }
        }

        public static void DelayWhenAvailable(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        public static bool CheckBalance(SBBState state, List<SBBTransaction> transactions)
        {
            int currentBalance = 0;
            foreach (var block in state.Blocks)
            {
                foreach (var transaction in block.HashedContent.Transactions)
                {
                    if (transaction.HashedContent.FromAccount == state.Wallet.PublicKey)
                    {
                        currentBalance--;
                    }
                    if (transaction.HashedContent.ToAccount == state.Wallet.PublicKey)
                    {
                        currentBalance++;
                    }
                }
            }

            foreach (var transaction in transactions)
            {
                if (transaction.HashedContent.FromAccount == state.Wallet.PublicKey)
                {
                    currentBalance--;
                }
                if (transaction.HashedContent.FromAccount == state.Wallet.PublicKey)
                {
                    currentBalance++;
                }
            }

            return currentBalance > 0;
        }
    }
}
