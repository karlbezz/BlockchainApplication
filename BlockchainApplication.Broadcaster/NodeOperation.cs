using BlockchainApplication.Data.Objects;
using BlockchainApplication.Protocol.Commands;
using BlockchainApplication.Protocol.Parser;
using BlockchainApplication.Protocol.Processor;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace BlockchainApplication.Broadcaster
{
    public class NodeOperation
    {
        private List<NodeDetails> seedNodes;
        private State state;
        private string sourceIp;
        private int sourcePort;
        private List<Command> incomingCommandRequests;

        public NodeOperation(List<NodeDetails> seedNodes, string sourceIp, int sourcePort)
        {
            this.seedNodes = seedNodes;
            this.sourceIp = sourceIp;
            this.sourcePort = sourcePort;
            this.state = new State();
            this.incomingCommandRequests = new List<Command>();
        }

        public void SyncData()
        {
            state.Syncing = true;
            int highestTransaction = 0;
            NodeDetails highestTransactionNode = null;
            foreach (var node in seedNodes)
            {
                string[] messageParams = new string[] { };
                byte[] message = MessageProcessor.ProcessMessage(BlockchainCommands.HIGHEST_TRN, messageParams);
                NodeBroadcasting.BroadcastToSeedNode(message, node);
                var command = (HighestTransactionResultCommand)NodeReceiving.ReceiveMessage(node, BlockchainCommands.HIGHEST_TRN_RES, sourcePort);
                if(highestTransaction < command.TransactionNumber)
                {
                    highestTransaction = command.TransactionNumber;
                    highestTransactionNode = node;
                }
            }

            int currentTxNumber = state.Transactions.Max(p => p.Number);
            for(int i = currentTxNumber; i < highestTransaction; i++)
            {
                byte[] message = MessageProcessor.ProcessMessage(BlockchainCommands.GET_TRANS, new string[] { currentTxNumber.ToString() });
                NodeBroadcasting.BroadcastToSeedNode(message, highestTransactionNode);
                var command = (NewTransactionCommand)NodeReceiving.ReceiveNewTransaction(highestTransactionNode, i, sourcePort);
                state.Transactions.Add(new Transaction(command.TransactionNumber, command.FromUser, command.ToUser, command.Timestamp));
            }

            state.Transactions.OrderBy(p => p.Number);
            state.Syncing = false;
        }
    }
}
