using BlockchainApplication.Broadcaster;
using BlockchainApplication.Data.Objects;
using BlockchainApplication.Data.Utilities;
using BlockchainApplication.SBB.Protocol.Commands;
using BlockchainApplication.SBB.Protocol.Creator;
using BlockchainApplication.SBB.Protocol.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BlockchainApplication.SBB
{
    public static class RequestsProcessor
    {
        public static byte[] ReceiveBytePacket(UdpClient udpServer, IPEndPoint remoteEp)
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

        public static Command ProcessRequest(byte[] messageBytes)
        {
            return MessageParser.ParseMessage(messageBytes);
        }

        public static void ProcessGetCountCommandRequest(SBBState nodeState, NodeDetails seedNode)
        {
            CountCommand countCommand = new CountCommand()
            {
                Blocks = nodeState.Blocks.Count
            };
            var message = MessageCreator.CreateMessage(countCommand);
            NodeBroadcasting.BroadcastToSeedNode(message, seedNode);
        }
        
        public static int ProcessCountCommandRequest(CountCommand command, int maxNodeCount)
        {
            return command.Blocks > maxNodeCount ? command.Blocks : maxNodeCount;
        }

        public static void ProcessGetBlockHashesCommandRequest(SBBState nodeState, NodeDetails seedNode)
        {
            BlockHashesCommand blockHashesCommand = new BlockHashesCommand()
            {
                Hashes = nodeState.Blocks.Select(p => p.Hash).ToList()
            };

            var message = MessageCreator.CreateMessage(blockHashesCommand);
            NodeBroadcasting.BroadcastToSeedNode(message, seedNode);
        }

        public static void ProcessRequestBlockCommandRequest(SBBState nodeState, NodeDetails seedNode, RequestBlockCommand requestBlockCommand)
        {
            BlockCommand blockCommand = new BlockCommand()
            {
                Block = nodeState.Blocks.First(p => p.Hash == requestBlockCommand.Hash)
            };

            var message = MessageCreator.CreateMessage(blockCommand);
            NodeBroadcasting.BroadcastToSeedNode(message, seedNode);
        }

        public static SBBState ProcessNewBlockRequest(SBBState nodeState, NewBlockCommand newBlockCommand)
        {
            string data = JsonConvert.SerializeObject(newBlockCommand.Block.HashedContent);
            string hash = Hashing.GenerateHash(data);
            if(newBlockCommand.Block.Hash != hash)
            {
                throw new Exception("Invalid hash");
            }
            nodeState.Blocks.Add(newBlockCommand.Block);
            return nodeState;
        }

        public static List<string> ProcessBlockHashesCommandRequest(BlockHashesCommand blockHashesCommand)
        {
            return blockHashesCommand.Hashes;
        }


        public static void ProcessRequestPacket(SBBState state, NodeDetails seedNode, Command responseCommand)
        {
            switch (responseCommand.CommandType)
            {
                case CommandType.GET_COUNT:
                    ProcessGetCountCommandRequest(state, seedNode);
                    break;
                case CommandType.GET_BLOCK_HASHES:
                    ProcessGetBlockHashesCommandRequest(state, seedNode);
                    break;
                case CommandType.REQ_BLOCK:
                    ProcessRequestBlockCommandRequest(state, seedNode, (RequestBlockCommand)responseCommand);
                    break;
                default:
                    break;
            }
        }
    }
}
