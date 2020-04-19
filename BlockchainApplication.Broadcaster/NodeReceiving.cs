using BlockchainApplication.Data.Objects;
using BlockchainApplication.Protocol.Commands;
using BlockchainApplication.Protocol.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Broadcaster
{
    public static class NodeReceiving
    {
        public static Command ReceiveNewTransaction(NodeDetails node, int expectedTxNumber, int sourcePort)
        {
            while (true)
            {
                UdpClient udpServer = new UdpClient(sourcePort);
                IPAddress address = IPAddress.Parse(node.IP);
                var remoteEp = new IPEndPoint(address, node.Port);
                byte[] messageBytes = udpServer.Receive(ref remoteEp);
                var command = CommandsParser.ParseCommand(messageBytes);

                if (command.CommandType.Equals(BlockchainCommands.NEW_TRANS))
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

        public static Command ReceiveMessage(NodeDetails node, BlockchainCommands commandType, int sourcePort)
        {
            while (true)
            {
                UdpClient udpServer = new UdpClient(sourcePort);
                IPAddress address = IPAddress.Parse(node.IP);
                var remoteEp = new IPEndPoint(address, node.Port);
                byte[] messageBytes = udpServer.Receive(ref remoteEp);
                var command = CommandsParser.ParseCommand(messageBytes);
                if (command.CommandType.Equals(commandType))
                {
                    return command;
                }
                else
                {
                    NodeBroadcasting.SendNotOkMessage(node);
                }
            }
        }
    }
}
