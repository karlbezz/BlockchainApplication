using BlockchainApplication.Data.Constants;
using BlockchainApplication.Data.Objects;
using BlockchainApplication.Protocol.Commands;
using BlockchainApplication.Protocol.Parser;
using BlockchainApplication.Protocol.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainApplication.Broadcaster
{
    public class NodeOperation
    {
        private List<NodeDetails> seedNodes;
        private State state;
        private int sourcePort;
        private bool firstRun;
        private bool processingUserCommand;
        private UdpClient udpServer;

        public NodeOperation(List<NodeDetails> seedNodes, int sourcePort, string username)
        {
            this.seedNodes = seedNodes;
            this.sourcePort = sourcePort;
            this.state = new State(username);
            this.firstRun = true;
            this.udpServer = new UdpClient(sourcePort);
            this.udpServer.Client.ReceiveTimeout = 15000;
            this.udpServer.Client.SendTimeout = 15000;
            this.processingUserCommand = false;
        }

        
        public void HandleUserRequests()
        {
            Console.WriteLine(PromptConstants.FormulatePromptDialog());
            string command = Console.ReadLine();
            if (command.StartsWith("newTx"))
            {
                string toAddress = command.Split(' ')[1];
                Console.WriteLine($"Adding Transaction.. Please wait");
                var transaction = AddNewTransaction(toAddress);
                if(transaction != null)
                {
                    string transactionOutput = PromptConstants.FormulateTransactionOutput(transaction);
                    Console.WriteLine($"Transaction Added\n{transactionOutput}");
                }
            }
            else
            {
                UserRequestsHandling.HandleUserRequest(state, command);
            }

            Console.WriteLine($"Press any key to continue..");
            Console.ReadLine();
            Console.Clear();
        }

        private Transaction AddNewTransaction(string to)
        {
            int transactionNumber = state.Transactions.Max(p => p.Number) + 1;
            if (!state.Balances.ContainsKey(to))
            {
                Console.WriteLine($"User {to} does not exist.");
                return null;
            }
            else if(state.Balances[state.Username] == 0)
            {
                Console.WriteLine($"Not enough balance for user {state.Username}.");
                return null;
            }

            while (!state.NodeState.Equals(NodeState.AVAILABLE))
            {
            }

            state.NodeState = NodeState.SENDING;
            Transaction transaction = new Transaction(transactionNumber, state.Username, to, (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            state.Transactions.Add(transaction);
            state.Balances[state.Username] -= 1;
            if (!state.Balances.ContainsKey(to))
            {
                state.Balances.Add(to, 0);
            }

            state.Balances[to] += 1;
            foreach(var node in seedNodes)
            {
                byte[] message = MessageProcessor.ProcessMessage(BlockchainCommands.NEW_TRANS,
                                                                     new string[] { transaction.Number.ToString(), transaction.From,
                                                                                    transaction.To, transaction.Timestamp.ToString()});
                NodeBroadcasting.BroadcastToSeedNode(message, node);
            }

            state.NodeState = NodeState.AVAILABLE;
            return transaction;
        }

        public void AddInitTransactions()
        {
            state.OutputLog.Add($"{DateTime.Now} - Adding mined transactions..");
            if (!state.Balances.ContainsKey(state.Username))
            {
                state.Balances.Add(state.Username, 0);
            }
            int maxTxNumber = state.Transactions.Count == 0 ? 1 : state.Transactions.Last().Number + 1;
            for(int i= 0; i < 10; i++)
            {
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                Transaction transaction = new Transaction(maxTxNumber, "00", state.Username, (int)(DateTime.UtcNow - dtDateTime).TotalSeconds);
                state.Transactions.Add(transaction);
                state.Balances[state.Username] += 1;
                maxTxNumber++;

                foreach(var node in seedNodes)
                {
                    byte[] message = MessageProcessor.ProcessMessage(BlockchainCommands.NEW_TRANS, 
                                                                     new string[] { transaction.Number.ToString(), transaction.From,
                                                                                    transaction.To, transaction.Timestamp.ToString()});
                    NodeBroadcasting.BroadcastToSeedNode(message, node);
                }
            }
            state.OutputLog.Add($"{DateTime.Now} - Mined transactions added.");
        }

        public void InitOperations()
        {
            Task syncTask = Task.Factory.StartNew(() => {
                while (true)
                {
                    while (!state.NodeState.Equals(NodeState.AVAILABLE))
                    {
                    }
                    SyncData();
                    if (firstRun)
                    {
                        AddInitTransactions();
                        firstRun = false;
                    }
                    Thread.Sleep(5000);
                }
            });

            Task handleRequests = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                while (true)
                {
                    ReceiveRequests();
                }
            });

            Task userRequestsHandling = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                while (true)
                {
                    HandleUserRequests();
                }
            });

            Task.WaitAll(syncTask, handleRequests, userRequestsHandling);
        }

        private void SyncData()
        {
            state.NodeState = NodeState.SYNCING;
            state = SyncingOperations.PerformSyncing(state, udpServer, sourcePort, seedNodes);
            state.NodeState = NodeState.AVAILABLE;
        }

        private void ReceiveRequests()
        {
            if (state.NodeState.Equals(NodeState.AVAILABLE))
            {
                state.NodeState = NodeState.RECEIVING;
                for (int i = 0; i < seedNodes.Count; i++)
                {
                    var remoteEp = new IPEndPoint(IPAddress.Any, seedNodes[i].Port);
                    byte[] messageBytes = ReceiveBytePacket(udpServer, remoteEp);
                    if (messageBytes.Length != 0)
                    {
                        RequestsProcessor.ProcessReceiveRequest(state, messageBytes, seedNodes[i]);
                    }
                }
                state.NodeState = NodeState.AVAILABLE;
                Thread.Sleep(6000);
            }
        }

        private byte[] ReceiveBytePacket(UdpClient udpServer, IPEndPoint remoteEp)
        {
            try
            {
                var received = udpServer.Receive(ref remoteEp).ToList();
                return received.ToArray();
            }
            catch (SocketException)
            {
                return new byte[0];
            }
        }
    }
}
