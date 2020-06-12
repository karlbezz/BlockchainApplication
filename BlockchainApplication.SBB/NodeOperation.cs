using BlockchainApplication.Broadcaster;
using BlockchainApplication.Data;
using BlockchainApplication.Data.Constants;
using BlockchainApplication.Data.Objects;
using BlockchainApplication.SBB.Protocol.Commands;
using BlockchainApplication.SBB.Protocol.Creator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BlockchainApplication.SBB
{
    public class NodeOperation
    {
        public SBBState NodeState;
        private UdpClient udpServer;
        private List<NodeDetails> seedNodes;
        private List<SBBTransaction> transactions;

        public NodeOperation(string ip, string port, string seedNodesDirectory)
        {
            this.NodeState = new SBBState(ip, port);
            this.udpServer = new UdpClient(NodeState.Port);
            this.transactions = new List<SBBTransaction>();
            udpServer.Client.ReceiveTimeout = 5000;
            udpServer.Client.SendTimeout = 5000;

            this.seedNodes = FileHandling.LoadSeedNodesFromCsv(seedNodesDirectory, NodeState.IP, NodeState.Port);
        }

        public void ListenToRequests()
        {
            while (true)
            {
                try
                {
                    Utilities.Delay(NodeState);
                    foreach (var seedNode in seedNodes)
                    {
                        NodeState.NodeState = SBBNodeState.PROCESSINGREQ;
                        NodeState.Logger.Add($"{DateTime.Now} - Listening to requests from node with address {seedNode.IP}:{seedNode.Port}");
                        var remoteEp = new IPEndPoint(IPAddress.Any, seedNode.Port);
                        byte[] messageBytes = RequestsProcessor.ReceiveBytePacket(udpServer, remoteEp);
                        if (messageBytes.Length != 0)
                        {
                            var responseCommand = RequestsProcessor.ProcessRequest(messageBytes);
                            if (responseCommand.CommandType == CommandType.NEW_BLOCK)
                            {
                                NewBlockCommand newBlockCommand = (NewBlockCommand)responseCommand;
                                Utilities.ConfirmBlock(newBlockCommand.Block);
                            }
                            else
                            {
                                RequestsProcessor.ProcessRequestPacket(NodeState, seedNode, responseCommand);
                            }
                        }
                        NodeState.Logger.Add($"{DateTime.Now} - Done listening. {seedNode.IP}:{seedNode.Port}");
                        NodeState.NodeState = SBBNodeState.AVAILABLE;
                        Utilities.DelayWhenAvailable(2000);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void PerformSyncingAndMineBlock()
        {
            bool firstRun = true;
            while (true)
            {
                try
                {
                    Utilities.Delay(NodeState);
                    NodeState.NodeState = SBBNodeState.MININGBLOCK;
                    NodeState.Logger.Add($"{DateTime.Now} - Started syncing and mining process.");
                    NodeState = MiningProcess.StartProcess(NodeState, seedNodes, udpServer, transactions, firstRun);
                    transactions = new List<SBBTransaction>();
                    NodeState.NodeState = SBBNodeState.AVAILABLE;
                    Thread.Sleep(10000);
                    firstRun = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        #region UserRequests 
        public void AcceptUserRequests()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Input command.");
                    string command = Console.ReadLine();
                    HandleUserRequest(command);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private void HandleUserRequest(string command)
        {
            if (command.Equals("help"))
            {
                HandleHelpRequest();
            }
            else if (command.Equals("listBlks"))
            {
                HandleListBlocksRequest();
            }
            else if (command.Equals("log"))
            {
                HandleOutputLogRequest();
            }
            else if (command.StartsWith("getBlk"))
            {
                HandleBlockRequest(command);
            }
            else if (command.StartsWith("newTx"))
            {
                HandleNewTransactionRequest(command);
            }
            else
            {
                Console.WriteLine("Invalid Command");
            }
        }

        private void HandleHelpRequest()
        {
            Console.WriteLine(PromptConstants.FormulateSBBPromptDialog());
        }

        private void HandleOutputLogRequest()
        {
            foreach(var logEntry in NodeState.Logger)
            {
                Console.WriteLine(logEntry);
            }
        }

        private void HandleBlockRequest(string command)
        {
            string blockHash = command.Split(' ')[1];
            Block block = NodeState.Blocks.FirstOrDefault(p => p.Hash == blockHash);
            Console.WriteLine(PromptConstants.SerializeAndOutputBlock(blockHash, block));
        }

        private void HandleListBlocksRequest()
        {
            Console.WriteLine(JsonConvert.SerializeObject(NodeState.Blocks.Select(p => p.Hash).ToList()));
        }

        private void HandleNewTransactionRequest(string command)
        {
            string toAddress = command.Split(' ')[1];
            Console.WriteLine($"Currently mining block, please wait");
            Utilities.Delay(NodeState);
            Console.WriteLine("Adding block to mining pool..");
            bool balanceAvailable = Utilities.CheckBalance(NodeState, transactions);
            if (balanceAvailable)
            {
                SBBTransaction transaction = TransactionGeneration.GenerateUserDefinedTransaction(NodeState.Wallet.PublicKey, toAddress);
                transactions.Add(transaction);
                Console.WriteLine($"Transaction added to mining pool.");
            }
            else
            {
                Console.WriteLine($"Not enough balance to satisfy user request.");
            }
        }
        #endregion UserRequests 
    }
}
