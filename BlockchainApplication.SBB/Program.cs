using BlockchainApplication.Broadcaster;
using BlockchainApplication.Data;
using BlockchainApplication.Data.Objects;
using BlockchainApplication.Data.Utilities;
using BlockchainApplication.SBB.Protocol.Commands;
using BlockchainApplication.SBB.Protocol.Creator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainApplication.SBB
{
    public class Program
    {
        static List<SBBTransaction> transactions = new List<SBBTransaction>();
        static List<NodeDetails> nodes = new List<NodeDetails>();
        static void Main(string[] args)
        {
            NodeOperation nodeOperation = new NodeOperation(args[0], args[1], args[2]);

            Task miningTask = Task.Factory.StartNew(() =>
            {
                nodeOperation.PerformSyncingAndMineBlock();
            });

            Task userRequestsTask = Task.Factory.StartNew(() =>
            {
                nodeOperation.AcceptUserRequests();
            });

            Task listenToRequestsTask = Task.Factory.StartNew(() =>
            {
                Utilities.DelayWhenAvailable(2000);
                nodeOperation.ListenToRequests();
            });

            Task.WaitAll(miningTask, userRequestsTask);
        }
    }
}
