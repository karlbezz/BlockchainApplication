using BlockchainApplication.Data.Objects;
using BlockchainApplication.Protocol.Commands;
using BlockchainApplication.Protocol.Parser;
using BlockchainApplication.Protocol.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Broadcaster
{
    public static class RequestsProcessor
    {
        public static State ProcessReceiveRequest(State state, byte[] messageBytes, NodeDetails node)
        {
            var command = CommandsParser.ParseCommand(messageBytes);
            return ProcessReceiveRequest(state, command, node);
        }

        public static State ProcessReceiveRequest(State state, Command command, NodeDetails node)
        {
            state.OutputLog.Add($"{DateTime.Now} - Received {command.CommandType} command.");
            if (state.NodeState != NodeState.SYNCING && command.CommandType.Equals(BlockchainCommands.NEW_TRANS))
            {
                state = ProcessNewTransactionReceived(state, command, node);
            }
            else if (command.CommandType.Equals(BlockchainCommands.GET_TRANS))
            {
                ProcessGetTransactionRequest(state, command, node);
            }
            else if (command.CommandType.Equals(BlockchainCommands.HIGHEST_TRN))
            {
                ProcessHighestTransactionRequest(state, command, node);
            }
            state.OutputLog.Add($"{DateTime.Now} - Command processed.");
            return state;
        }

        public static void ProcessHighestTransactionRequest(State state, Command command, NodeDetails node)
        {
            int transactionNumber = state.Transactions.Count == 0 ? 0 : state.Transactions.Max(p => p.Number);
            byte[] message = MessageProcessor.ProcessMessage(BlockchainCommands.HIGHEST_TRN_RES,
                                                             new string[] { transactionNumber.ToString() });
            NodeBroadcasting.BroadcastToSeedNode(message, node);
        }

        public static void ProcessGetTransactionRequest(State state, Command command, NodeDetails node)
        {
            var getTransactionCommand = (GetTransactionCommand)command;
            var transaction = state.Transactions.FirstOrDefault(p => p.Number == getTransactionCommand.TransactionNumber);
            if (transaction == null)
            {
                NodeBroadcasting.SendNotOkMessage(node);
            }
            else
            {
                byte[] message = MessageProcessor.ProcessMessage(BlockchainCommands.NEW_TRANS, new string[]
                {
                        transaction.Number.ToString(), transaction.From, transaction.To, transaction.Timestamp.ToString()
                });

                NodeBroadcasting.BroadcastToSeedNode(message, node);
            }
        }

        public static State ProcessNewTransactionReceived(State state, Command command, NodeDetails node)
        {
            var newTransactionCommand = (NewTransactionCommand)command;
            var existingTransaction = state.Transactions.FirstOrDefault(p => p.Number == newTransactionCommand.TransactionNumber);
            if (existingTransaction == null)
            {
                state.Transactions.Add(new Transaction(newTransactionCommand.TransactionNumber, newTransactionCommand.FromUser,
                                                       newTransactionCommand.ToUser, newTransactionCommand.Timestamp));
            }
            else
            {
                int index = state.Transactions.FindIndex(p => p.Number == newTransactionCommand.TransactionNumber);
                if (existingTransaction.Timestamp > newTransactionCommand.Timestamp)
                {
                    //Replace transaction
                    state.Balances[state.Transactions[index].To] -= 1;
                    state.Transactions[index] = new Transaction(newTransactionCommand.TransactionNumber, newTransactionCommand.FromUser,
                                                                newTransactionCommand.ToUser, newTransactionCommand.Timestamp);
                    if (!state.Balances.ContainsKey(newTransactionCommand.ToUser))
                    {
                        state.Balances.Add(newTransactionCommand.ToUser, 0);
                    }
                    state.Balances[newTransactionCommand.ToUser] += 1;
                }
            }

            NodeBroadcasting.SendOkMessage(node);
            return state;
        }
    }
}
