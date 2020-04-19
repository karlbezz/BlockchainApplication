using BlockchainApplication.Broadcaster;
using BlockchainApplication.Data;
using BlockchainApplication.Data.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlockchainApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string ip = args[0];
            int listeningPort = int.Parse(args[1]);
            string filePath = args[2];

            Console.WriteLine($"{DateTime.Now} - Loading Seed Nodes..");
            List<NodeDetails> seedNodes = FileHandling.LoadSeedNodesFromCsv(filePath, ip, listeningPort);
            Console.WriteLine($"{DateTime.Now} - Seed Nodes Loaded");

            Random r = new Random(DateTime.Now.Millisecond);
            int randomTicks = r.Next(500, 5000);
            Console.WriteLine($"{DateTime.Now} - Random ticks: {randomTicks}");
            MessageHandling messageHandling = new MessageHandling(ip, listeningPort, seedNodes);

            Task listeningTask = Task.Factory.StartNew(() =>
            {
                messageHandling.ProcessIncomingMessages(listeningPort);
            });

            Task broadcastingTask = Task.Factory.StartNew(() =>
            {
                messageHandling.BroadcastMessages(randomTicks);
            });

            Task.WaitAll(listeningTask, broadcastingTask);
        }
    }
}
