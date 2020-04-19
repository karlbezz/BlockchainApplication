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
        private UdpClient udpServer;

        public NodeOperation(List<NodeDetails> seedNodes, int sourcePort, string username)
        {
            this.seedNodes = seedNodes;
            this.sourcePort = sourcePort;
            this.state = new State(username);
            this.firstRun = true;
            this.udpServer = new UdpClient(sourcePort);
            this.udpServer.Client.ReceiveTimeout = 5000;
            this.udpServer.Client.SendTimeout = 5000;
        }

        public void AddInitTransactions()
        {
            Console.WriteLine($"{DateTime.Now} - Adding mined transactions..");
            if (!state.Balances.ContainsKey(state.Username))
            {
                state.Balances.Add(state.Username, 0);
            }
            int maxTxNumber = state.Transactions.Count;
            for(int i= 0; i < 10; i++)
            {
                Transaction transaction = new Transaction(maxTxNumber, "00", state.Username, DateTime.Now.Ticks);
                state.Transactions.Add(transaction);
                state.Balances[state.Username] += 1;
                maxTxNumber++;
            }
            Console.WriteLine($"{DateTime.Now} - Mined transactions added.");
        }

        public void InitOperations()
        {
            while (true)
            {
                SyncData();
                if (firstRun)
                {
                    AddInitTransactions();
                    firstRun = false;
                }
                Task task = Task.Run(() =>
                {
                    ReceiveRequests();
                });
                task.Wait(TimeSpan.FromSeconds(5));
            }
        }

        private void SyncData()
        {
            state.NodeState = NodeState.SYNCING;
            Console.WriteLine($"{DateTime.Now} - Syncing..");
            int highestTransaction = 0;
            NodeDetails highestTransactionNode = null;
            foreach (var node in seedNodes)
            {
                string[] messageParams = new string[] { };
                byte[] message = MessageProcessor.ProcessMessage(BlockchainCommands.HIGHEST_TRN, messageParams);
                NodeBroadcasting.BroadcastToSeedNode(message, node);
                var command = SyncingOperations.ReceiveMessage(udpServer, node, BlockchainCommands.HIGHEST_TRN_RES, sourcePort);
                if(command.CommandType != BlockchainCommands.NO_RESPONSE)
                {
                    var highestTransactionResultCommand = (HighestTransactionResultCommand)command;
                    if (highestTransaction < highestTransactionResultCommand.TransactionNumber)
                    {
                        highestTransaction = highestTransactionResultCommand.TransactionNumber;
                        highestTransactionNode = node;
                    }
                }
            }

            int currentTxNumber = state.Transactions.Count == 0 ? 0 : state.Transactions.Max(p => p.Number);
            SyncingOperations.UpdateState(udpServer, state, highestTransactionNode, sourcePort, highestTransaction, currentTxNumber);

            state.Transactions.OrderBy(p => p.Number);
            Console.WriteLine($"{DateTime.Now} - Syncing Finished.");
            state.NodeState = NodeState.AVAILABLE;
        }

        private void ReceiveRequests()
        {
            seedNodes.ForEach(x => x.Online = true);
            while (true)
            {   
                for(int i = 0;i<seedNodes.Count; i++)
                {
                    if (seedNodes[i].Online)
                    {
                        var remoteEp = new IPEndPoint(IPAddress.Any, seedNodes[i].Port);
                        byte[] messageBytes = ReceiveBytePacket(udpServer, remoteEp);
                        if (messageBytes.Length == 0)
                        {
                            seedNodes[i].Online = false;
                        }
                        else
                        {
                            RequestsProcessor.ProcessReceiveRequest(state, messageBytes, seedNodes[i]);
                        }
                    }
                }
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
