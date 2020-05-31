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
    public class Program
    {
        static List<SBBTransaction> transactions = new List<SBBTransaction>();

        static void Main(string[] args)
        {
            //Generate Wallet
            SBBState state = new SBBState();
            Task miningTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Delay(state);
                    state.NodeState = SBBNodeState.MININGBLOCK;
                    MineNewBlock(state);
                    state.NodeState = SBBNodeState.AVAILABLE;
                    Console.WriteLine($"Block mined.\nNew Block:\n {JsonConvert.SerializeObject(state.Blocks.Last())}\n\n");
                    Thread.Sleep(30000);
                }
            });

            Task userRequestsTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Console.WriteLine("Input command.");
                    string command = Console.ReadLine();
                    HandleUserRequest(state, command);
                }
            });

            Task.WaitAll(miningTask, userRequestsTask);
        }

        static SBBState MineNewBlock(SBBState state)
        {
            Console.WriteLine($"Mining new block..");
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            int timestamp = (int)(DateTime.UtcNow - dtDateTime).TotalSeconds;
            string previousBlockHash = state.Blocks.Count > 0 ? state.Blocks.Last().Hash : "00";
            HashedContent hashedContent = new HashedContent(previousBlockHash, timestamp);
            if (transactions.Count == 0)
            {
                SBBTransaction transaction = TransactionGeneration.GenerateCoinbaseTransaction(state.Wallet.PublicKey);
                hashedContent.Transactions = new List<SBBTransaction>() { transaction };
            }
            else
            {
                hashedContent.Transactions = new List<SBBTransaction>(transactions);
            }

            Block block = new Block(hashedContent);
            block = Hashing.GenerateBlockHash(block, 4);
            state.Blocks.Add(block);
            transactions = new List<SBBTransaction>();
            return state;
        }

        static void HandleUserRequest(SBBState state, string command)
        {
            if (command.StartsWith("help"))
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine($"Available commands..");
                builder.AppendLine($"- help to popup this dialog");
                builder.AppendLine($"- getBlk [blkHash] to get the transaction");
                builder.AppendLine($"- newTx [to] to add a new transaction");
                Console.WriteLine(builder.ToString());
            }
            else if (command.StartsWith("getBlk"))
            {
                string blockHash = command.Split(' ')[1];
                Block block = state.Blocks.FirstOrDefault(p => p.Hash == blockHash);
                if(block != null)
                {
                    Console.WriteLine($"Requested Block:\n{JsonConvert.SerializeObject(block)}");
                }
                else
                {
                    Console.WriteLine($"Block with hash {blockHash} not found.");
                }
            }
            else if(command.StartsWith("newTx"))
            {
                string toAddress = command.Split(' ')[1];
                Console.WriteLine($"Currently mining block, please wait");
                Delay(state);
                Console.WriteLine("Adding block to mining pool..");
                bool balanceAvailable = CheckBalance(state);
                if (balanceAvailable)
                {
                    SBBTransaction transaction = TransactionGeneration.GenerateUserDefinedTransaction(state.Wallet.PublicKey, toAddress);
                    transactions.Add(transaction);
                    Console.WriteLine($"Transaction added to mining pool.");
                }
                else
                {
                    Console.WriteLine($"Not enough balance to satisfy user request.");
                }
            }
            else
            {
                Console.WriteLine($"Invalid command.");
            }
        }

        static void Delay(SBBState state)
        {
            while (state.NodeState != SBBNodeState.AVAILABLE)
            {
                Thread.Sleep(1000);
            }
        }

        static bool CheckBalance(SBBState state)
        {
            int currentBalance = 0;
            foreach(var block in state.Blocks)
            {
                foreach(var transaction in block.HashedContent.Transactions)
                {
                    if(transaction.From == state.Wallet.PublicKey)
                    {
                        currentBalance--;
                    }
                    if(transaction.To == state.Wallet.PublicKey)
                    {
                        currentBalance++;
                    }
                }
            }

            foreach(var transaction in transactions)
            {
                if(transaction.From == state.Wallet.PublicKey)
                {
                    currentBalance--;
                }
                if(transaction.To == state.Wallet.PublicKey)
                {
                    currentBalance++;
                }
            }

            return currentBalance > 0;
        }
    }
}
