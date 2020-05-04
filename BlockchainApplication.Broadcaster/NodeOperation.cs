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
        }


        public void HandleUserRequests()
        {
            Console.WriteLine(PromptConstants.FormulatePromptDialog());
            string command = Console.ReadLine();
            if (command.StartsWith("newTx"))
            {
                string[] splitCommand = command.Split(' ');
                string toAddress = splitCommand[1];
                int approved = int.Parse(splitCommand[2]);
                if (approved > 1 || approved < 0)
                {
                    throw new Exception("Invalid approval status");
                }
                Console.WriteLine($"Adding Transaction.. Please wait");
                while (!state.NodeState.Equals(NodeState.AVAILABLE))
                {
                }

                state.NodeState = NodeState.SENDING;
                state = UserRequestsHandling.HandleNewTransactionRequest(state, seedNodes, toAddress, approved);
                state.NodeState = NodeState.AVAILABLE;
            }
            else if (command.StartsWith("approveTx"))
            {
                string transactionNumber = command.Split(' ')[1];
                Transaction transaction = UserRequestsHandling.GetApprovalTransaction(state, transactionNumber);
                if(transaction != null)
                {
                    while (!state.NodeState.Equals(NodeState.AVAILABLE))
                    {
                    }

                    state.NodeState = NodeState.SENDING;
                    state = UserRequestsHandling.HandleNewApprovalTransaction(state, seedNodes, transaction);
                    state.NodeState = NodeState.AVAILABLE;
                }
                Console.WriteLine($"Approving Transaction.. Please wait");
            }
            else
            {
                UserRequestsHandling.HandleUserRequest(state, command);
            }

            Console.WriteLine($"Press any key to continue..");
            Console.ReadLine();
            Console.Clear();
        }

        public void InitOperations()
        {
            Task syncTask = Task.Factory.StartNew(() => {
                while (true)
                {
                    try
                    {
                        SyncData();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"An error occured while syncing data.");
                        Console.WriteLine(e);
                        throw e;
                    }
                    
                    Thread.Sleep(5000);
                }
            });

            Task handleRequests = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                while (true)
                {
                    try
                    {
                        ReceiveRequests();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"An error occured while receiving requests.");
                        Console.WriteLine(e);
                        throw e;
                    }
                }
            });

            Task userRequestsHandling = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                while (true)
                {
                    try
                    {
                        HandleUserRequests();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"An error occured while handling user requests.");
                        Console.WriteLine(e);
                        throw e;
                    }
                }
            });

            Task.WaitAll(syncTask, handleRequests, userRequestsHandling);
        }

        private void SyncData()
        {
            while (!state.NodeState.Equals(NodeState.AVAILABLE))
            {
            }
            state.NodeState = NodeState.SYNCING;
            state = SyncingOperations.PerformSyncing(state, udpServer, sourcePort, seedNodes);
            if (firstRun)
            {
                state = SyncingOperations.AddInitTransactions(state, seedNodes);
                firstRun = false;
            }
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
                    byte[] messageBytes = RequestsProcessor.ReceiveBytePacket(udpServer, remoteEp);
                    if (messageBytes.Length != 0)
                    {
                        RequestsProcessor.ProcessReceiveRequest(state, messageBytes, seedNodes[i]);
                    }
                }
                state.NodeState = NodeState.AVAILABLE;
                Thread.Sleep(6000);
            }
        }

    }
}
