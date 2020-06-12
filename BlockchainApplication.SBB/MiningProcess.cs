using BlockchainApplication.Broadcaster;
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
using System.Threading.Tasks;

namespace BlockchainApplication.SBB
{
    public static class MiningProcess
    {
        private static TimeElapsed RebroadcastAndElapseTime(byte[] message, NodeDetails seedNode, TimeElapsed timeElapsed)
        {
            NodeBroadcasting.BroadcastToSeedNode(message, seedNode);
            timeElapsed.CurrentTime = DateTime.Now;
            timeElapsed.Tries++;
            return timeElapsed;
        }

        public static SBBState StartProcess(SBBState nodeState, List<NodeDetails> seedNodes, UdpClient udpServer, List<SBBTransaction> transactions, bool firstRun = false)
        {
            nodeState.Logger.Add($"{DateTime.Now} - Syncing and scanning.");
            nodeState = ScanForNewBlocks(nodeState, seedNodes, udpServer);
            nodeState.Logger.Add($"{DateTime.Now} - Mining");
            nodeState = MineNewBlock(nodeState, transactions, firstRun, seedNodes);
            return nodeState;
        }

        private static SBBState MineNewBlock(SBBState state, List<SBBTransaction> transactions, bool firstRun, List<NodeDetails> seedNodes)
        {
            if(firstRun || transactions.Count != 0)
            {
                state.Logger.Add($"Mining new block..");
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                int timestamp = (int)(DateTime.UtcNow - dtDateTime).TotalSeconds;
                string previousBlockHash = state.Blocks.Count > 0 ? state.Blocks.Last().Hash : "0";
                HashedContent hashedContent = new HashedContent(previousBlockHash, timestamp);
                if (firstRun)
                {
                    SBBTransaction transaction = TransactionGeneration.GenerateCoinbaseTransaction(state.Wallet.PublicKey);
                    hashedContent.Transactions = new List<SBBTransaction>() { transaction, transaction, transaction, transaction, transaction };
                }
                else if (transactions.Count != 0)
                {
                    hashedContent.Transactions = new List<SBBTransaction>(transactions);
                }

                Block block = new Block(hashedContent);
                block = Hashing.GenerateBlockHash(block, 4);
                state.Blocks.Add(block);
                state.Logger.Add($"{DateTime.Now} - Block mined.\nNew Block:\n {JsonConvert.SerializeObject(block)}\n\n");
                TransmitBlockToSeeders(block, seedNodes);
            }

            return state;
        }

        private static void TransmitBlockToSeeders(Block block, List<NodeDetails> seedNodes)
        {
            NewBlockCommand newBlockCommand = new NewBlockCommand()
            {
                Block = block
            };

            byte[] messageBytes = MessageCreator.CreateMessage(newBlockCommand);
            foreach(var seedNode in seedNodes)
            {
                NodeBroadcasting.BroadcastToSeedNode(messageBytes, seedNode);
            }
        }

        private static SBBState ScanForNewBlocks(SBBState nodeState, List<NodeDetails> seedNodes, UdpClient udpServer)
        {
            byte[] message = MessageCreator.CreateMessage(new GetCountCommand());
            int maxCount = 0;
            var maxSeedNode = new NodeDetails();
            foreach (var seedNode in seedNodes)
            {
                var remoteEp = new IPEndPoint(IPAddress.Any, seedNode.Port);
                bool requestReceived = false;
                NodeBroadcasting.BroadcastToSeedNode(message, seedNode);
                TimeElapsed timeElapsed = new TimeElapsed();

                while (!requestReceived && timeElapsed.Tries < 5)
                {
                    byte[] messageBytes = RequestsProcessor.ReceiveBytePacket(udpServer, remoteEp);
                    if (messageBytes.Length == 0)
                    {
                        timeElapsed = RebroadcastAndElapseTime(message, seedNode, timeElapsed);
                    }
                    else
                    {
                        var responseCommand = RequestsProcessor.ProcessRequest(messageBytes);
                        nodeState.Logger.Add($"{DateTime.Now} - Received {responseCommand.CommandType} request");
                        switch (responseCommand.CommandType)
                        {
                            case CommandType.COUNT:
                                int count = RequestsProcessor.ProcessCountCommandRequest((CountCommand)responseCommand, maxCount);
                                maxSeedNode = count > maxCount ? seedNode : maxSeedNode;
                                maxCount = count;
                                requestReceived = true;
                                break;
                            case CommandType.NEW_BLOCK:
                                nodeState = RequestsProcessor.ProcessNewBlockRequest(nodeState, (NewBlockCommand)responseCommand);
                                break;
                            default:
                                RequestsProcessor.ProcessRequestPacket(nodeState, seedNode, responseCommand);
                                break;
                        }
                        nodeState.Logger.Add($"{DateTime.Now} - Request {responseCommand.CommandType} processed"); ;
                        if ((DateTime.Now - timeElapsed.CurrentTime).TotalSeconds > 7)
                        {
                            timeElapsed = RebroadcastAndElapseTime(message, seedNode, timeElapsed);
                        }
                    }
                }
            }

            nodeState.Logger.Add($"{DateTime.Now} - Scanning done. Blocks to sync {maxCount}");

            if (maxCount > nodeState.Blocks.Count)
            {
                List<string> hashes = GetBlockHashes(maxSeedNode, udpServer);
                nodeState = GetBlocks(maxSeedNode, hashes, nodeState, udpServer);
            }

            nodeState.Logger.Add($"{DateTime.Now} - Syncing finished.");
            return nodeState;
        }

        private static List<string> GetBlockHashes(NodeDetails seedNode, UdpClient udpServer)
        {
            List<string> blockHashes = new List<string>();
            GetBlockHashesCommand getBlockHashesCommand = new GetBlockHashesCommand();
            byte[] message = MessageCreator.CreateMessage(getBlockHashesCommand);
            NodeBroadcasting.BroadcastToSeedNode(message, seedNode);

            var remoteEp = new IPEndPoint(IPAddress.Any, seedNode.Port);
            bool requestReceived = false;
            TimeElapsed timeElapsed = new TimeElapsed();
            while (!requestReceived && timeElapsed.Tries < 5)
            {
                byte[] messageBytes = RequestsProcessor.ReceiveBytePacket(udpServer, remoteEp);
                if (messageBytes.Length == 0)
                {
                    timeElapsed = RebroadcastAndElapseTime(message, seedNode, timeElapsed);
                }
                else
                {
                    var responseCommand = RequestsProcessor.ProcessRequest(messageBytes);
                    switch (responseCommand.CommandType)
                    {
                        case CommandType.BLOCK_HASHES:
                            blockHashes = RequestsProcessor.ProcessBlockHashesCommandRequest((BlockHashesCommand)responseCommand);
                            requestReceived = true;
                            break;
                        default: break;
                    }

                    if ((DateTime.Now - timeElapsed.CurrentTime).TotalSeconds >= 7)
                    {
                        timeElapsed = RebroadcastAndElapseTime(message, seedNode, timeElapsed);
                    }
                }
            }

            return blockHashes;
        }

        private static SBBState GetBlocks(NodeDetails seedNode, List<string> hashes, SBBState nodeState, UdpClient udpServer)
        {
            foreach (var hash in hashes)
            {
                if(!nodeState.Blocks.Any(p=>p.Hash == hash))
                {
                    RequestBlockCommand requestBlockCommand = new RequestBlockCommand(hash);
                    byte[] message = MessageCreator.CreateMessage(requestBlockCommand);
                    NodeBroadcasting.BroadcastToSeedNode(message, seedNode);
                    bool requestReceived = false;
                    TimeElapsed timeElapsed = new TimeElapsed();
                    var remoteEp = new IPEndPoint(IPAddress.Any, seedNode.Port);
                    while (!requestReceived && timeElapsed.Tries < 5)
                    {
                        byte[] messageBytes = RequestsProcessor.ReceiveBytePacket(udpServer, remoteEp);
                        if (messageBytes.Length == 0)
                        {
                            timeElapsed = RebroadcastAndElapseTime(message, seedNode, timeElapsed);
                        }
                        else
                        {
                            var responseCommand = RequestsProcessor.ProcessRequest(messageBytes);
                            if (responseCommand.CommandType == CommandType.BLOCK)
                            {
                                var block = ((BlockCommand)responseCommand).Block;
                                bool blockConfirmed = Utilities.ConfirmBlock(block, hash);
                                if (blockConfirmed)
                                {
                                    nodeState.Blocks.Add(block);
                                    requestReceived = true;
                                }
                            }

                            if ((DateTime.Now - timeElapsed.CurrentTime).TotalSeconds >= 7)
                            {
                                timeElapsed = RebroadcastAndElapseTime(message, seedNode, timeElapsed);
                            }
                        }
                    }
                }
            }

            return nodeState;
        }
    }
}
