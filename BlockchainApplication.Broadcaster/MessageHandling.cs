using BlockchainApplication.Data;
using BlockchainApplication.Data.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainApplication.Broadcaster
{
    public class MessageHandling
    {
        private List<NodeDetails> seedNodes;
        private string sourceIp;
        private int sourcePort;

        public MessageHandling(string sourceIp, int sourcePort, List<NodeDetails> seedNodes)
        {
            this.seedNodes = seedNodes;
            this.sourceIp = sourceIp;
            this.sourcePort = sourcePort;
        }

        public void BroadcastMessages(int randomTicks)
        {
            while (true)
            {
                Console.WriteLine($"{DateTime.Now} - Broadcasting Messages..");
                foreach (var node in seedNodes)
                {
                    BroadcastMessage(node.IP, node.Port);
                }
                Console.WriteLine($"{DateTime.Now} - Messages Broadcasted.");
                Thread.Sleep(randomTicks);
            }
        }

        private void BroadcastMessage(string targetIp, int targetPort)
        {
            try
            {
                var client = new UdpClient();
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(targetIp), targetPort);
                client.Connect(ep);
                string message = $"Message [ Test ] from {sourceIp}:{sourcePort}";
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                client.Send(messageBytes, messageBytes.Length);
                client.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DateTime.Now} - An error has occured when attempting to broadcast message to {targetIp}:{targetPort}." +
                                  $"\n{DateTime.Now} - Error: {e.Message}");
            }
        }

        public void ProcessIncomingMessages(int sourcePort)
        {
            UdpClient udpServer = new UdpClient(sourcePort);

            while (true)
            {
                try
                {
                    var remoteEp = new IPEndPoint(IPAddress.Any, sourcePort);
                    byte[] messageBytes = udpServer.Receive(ref remoteEp);
                    string messageString = Encoding.ASCII.GetString(messageBytes);
                    Console.WriteLine(messageString);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{DateTime.Now} - An error has occured while processing incoming messsages: {e.Message}");
                }
            }
        }
    }
}
