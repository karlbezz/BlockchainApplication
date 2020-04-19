using BlockchainApplication.Data.Objects;
using BlockchainApplication.Protocol.Processor;
using System.Net;
using System.Net.Sockets;

namespace BlockchainApplication.Broadcaster
{
    public static class NodeBroadcasting
    {
        public static void BroadcastToSeedNode(byte[] message, NodeDetails node)
        {
            var client = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(node.IP), node.Port);
            client.Connect(ep);
            client.Send(message, message.Length);
            client.Close();
        }

        public static void SendOkMessage(NodeDetails node)
        {
            byte[] okMessage = MessageProcessor.ProcessMessage(Protocol.Commands.BlockchainCommands.OK_MSG, new string[] { });
            BroadcastToSeedNode(okMessage, node);
        }

        public static void SendNotOkMessage(NodeDetails node)
        {
            byte[] notOkMessage = MessageProcessor.ProcessMessage(Protocol.Commands.BlockchainCommands.NOK_MSG, new string[] { });
            BroadcastToSeedNode(notOkMessage, node);
        }
    }
}
