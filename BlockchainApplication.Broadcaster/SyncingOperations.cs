﻿using BlockchainApplication.Data.Objects;
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

        public static State PerformSyncing(State state, UdpClient udpServer, int sourcePort, List<NodeDetails> seedNodes)
        {
            state.OutputLog.Add($"{DateTime.Now} - Syncing..");
            int highestTransaction = 1;
            NodeDetails highestTransactionNode = null;

            string[] messageParams = new string[] { };
            byte[] message = MessageProcessor.ProcessMessage(BlockchainCommands.HIGHEST_TRN, messageParams);
            foreach (var node in seedNodes)
            {
                NodeBroadcasting.BroadcastToSeedNode(message, node);
                var command = SyncingOperations.ReceiveMessage(state, udpServer, node, BlockchainCommands.HIGHEST_TRN_RES, sourcePort);
                if (command.CommandType != BlockchainCommands.NO_RESPONSE)
                {
                    var highestTransactionResultCommand = (HighestTransactionResultCommand)command;
                    if (highestTransaction < highestTransactionResultCommand.TransactionNumber)
                    {
                        state.OutputLog.Add($"{DateTime.Now} - Highest Transaction Updated.");
                        highestTransaction = highestTransactionResultCommand.TransactionNumber;
                        highestTransactionNode = node;
                    }
                }
            }

            int currentTxNumber = state.Transactions.Count == 0 ? 1 : state.Transactions.Max(p => p.Number);
            if (currentTxNumber < highestTransaction)
            {
                UpdateState(udpServer, state, highestTransactionNode, sourcePort, highestTransaction, currentTxNumber);
                state.Transactions.OrderBy(p => p.Number);
            }

            state.OutputLog.Add($"{DateTime.Now} - Syncing Finished.");
            return state;
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
                }
            }
        }

        public static Command ReceiveMessage(State state, UdpClient udpServer, NodeDetails node, BlockchainCommands commandType, int sourcePort)
        {
            while (true)
            {
                var command = ReceiveCommand(udpServer, node, sourcePort);
                state.OutputLog.Add($"{DateTime.Now} - Received {command.CommandType} command.");
                if (command.CommandType.Equals(commandType) || command.CommandType.Equals(BlockchainCommands.NO_RESPONSE))
                {
                    return command;
                }
                else
                {
                    RequestsProcessor.ProcessReceiveRequest(state, command, node);
                }
            }
        }

        private static State UpdateState(UdpClient udpServer, State state, NodeDetails node, int sourcePort, int highestTransaction, int currentTransaction)
        {
            for (int i = currentTransaction; i <= highestTransaction; i++)
            {
                state.OutputLog.Add($"{DateTime.Now} - Getting Transaction {i}..");
                byte[] message = MessageProcessor.ProcessMessage(BlockchainCommands.GET_TRANS, new string[] { i.ToString() });
                int retry = 0;
                var command = new NewTransactionCommand();
                while(retry < 5)
                {
                    NodeBroadcasting.BroadcastToSeedNode(message, node);
                    var newCommand = ReceiveNewTransaction(udpServer, node, i, sourcePort);
                    if (newCommand.CommandType.Equals(BlockchainCommands.NEW_TRANS))
                    {
                        command = (NewTransactionCommand)newCommand;
                        break;
                    }
                    retry++;
                }
                
                if (retry >= 5)
                {
                    state.OutputLog.Add($"{DateTime.Now} - Failed to get transaction {i}");
                    return state;
                }
                state.Transactions.Add(new Transaction(command.TransactionNumber, command.FromUser, command.ToUser, command.Timestamp));
                if (!state.Balances.ContainsKey(command.ToUser))
                {
                    state.Balances.Add(command.ToUser, 0);
                }
                state.Balances[command.ToUser] += 1;

                if (state.Balances.ContainsKey(command.FromUser))
                {
                    state.Balances[command.FromUser] -= 1;
                }
            }

            return state;
        }
    }
}
