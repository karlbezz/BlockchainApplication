using BlockchainApplication.Data.Objects;
using BlockchainApplication.Protocol.Commands;
using BlockchainApplication.Protocol.Parser;
using BlockchainApplication.Protocol.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Broadcaster
{
    public static class SyncingOperations
    {
        public static Command ReceiveCommand(UdpClient udpServer, NodeDetails node, int sourcePort)
        {
            IPAddress address = IPAddress.Parse(node.IP);
            var remoteEp = new IPEndPoint(address, node.Port);
            var command = ReceivePacket(udpServer, remoteEp);
            return ReceivePacket(udpServer, remoteEp);
        }

        private static Command ReceivePacket(UdpClient udpServer, IPEndPoint remoteEp)
        {
            try
            {
                var received = udpServer.Receive(ref remoteEp);
                var command = CommandsParser.ParseCommand(received);
                return command;
            }
            catch (SocketException)
            {
                return new NoResponseCommand()
                {
                    CommandType = BlockchainCommands.NO_RESPONSE
                };
            }
        }

        private static Command ReceiveNewTransaction(UdpClient udpServer, NodeDetails node, int expectedTxNumber, int sourcePort)
        {
            while (true)
            {
                var command = ReceiveCommand(udpServer, node, sourcePort);
                if (command.CommandType.Equals(BlockchainCommands.NO_RESPONSE))
                {
                    return command;
                }
                else if (command.CommandType.Equals(BlockchainCommands.NEW_TRANS))
                {
                    var newTransactionCommand = (NewTransactionCommand)command;
                    if (newTransactionCommand.TransactionNumber == expectedTxNumber)
                    {
                        return command;
                    }
                    else
                    {
                        NodeBroadcasting.SendNotOkMessage(node);
                    }
                }
                else
                {
                    NodeBroadcasting.SendNotOkMessage(node);
                }
            }
        }

        public static Command ReceiveMessage(UdpClient udpServer, NodeDetails node, BlockchainCommands commandType, int sourcePort)
        {
            while (true)
            {
                var command = ReceiveCommand(udpServer, node, sourcePort);
                if (command.CommandType.Equals(commandType) || command.CommandType.Equals(BlockchainCommands.NO_RESPONSE))
                {
                    return command;
                }
                else
                {
                    NodeBroadcasting.SendNotOkMessage(node);
                }
            }
        }

        public static State UpdateState(UdpClient udpServer, State state, NodeDetails node, int sourcePort, int highestTransaction, int currentTransaction)
        {
            for (int i = currentTransaction; i < highestTransaction; i++)
            {
                byte[] message = MessageProcessor.ProcessMessage(BlockchainCommands.GET_TRANS, new string[] { currentTransaction.ToString() });
                NodeBroadcasting.BroadcastToSeedNode(message, node);
                var command = (NewTransactionCommand)ReceiveNewTransaction(udpServer, node, i, sourcePort);
                if (command.CommandType.Equals(BlockchainCommands.NO_RESPONSE))
                {
                    return state;
                }
                state.Transactions.Add(new Transaction(command.TransactionNumber, command.FromUser, command.ToUser, command.Timestamp));
            }

            return state;
        }
    }
}
