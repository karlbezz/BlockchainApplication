using BlockchainApplication.Data.Constants;
using BlockchainApplication.Data.Objects;
using BlockchainApplication.Protocol.Commands;
using BlockchainApplication.Protocol.Processor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainApplication.Broadcaster
{
    public static class UserRequestsHandling
    {
        public static void HandleUserRequest(State state, string command)
        {
            if (command.StartsWith("help"))
            {
                HandleHelpRequest();
            }
            else if (command.StartsWith("log"))
            {
                HandleLogRequest(state);   
            }
            else if (command.StartsWith("getTx"))
            {
                HandleGetTransactionRequest(state, command);
            }
            else if (command.StartsWith("highTx"))
            {
                HandleHighTransactionRequest(state);
            }
            else if (command.StartsWith("listTx"))
            {
                HandleListTransactionsRequest(state);
            }
            else if (command.StartsWith("balances"))
            {
                HandleBalancesRequest(state);
            }
            else if (command.StartsWith("balance"))
            {
                HandleBalanceRequest(state);
            }
            else
            {
                HandleInvalidRequest();
            }
        }       
        
        private static void HandleBalanceRequest(State state)
        {
            Console.WriteLine($"Balance for user {state.Username}: {state.Balances[state.Username]}");
        }

        private static void HandleBalancesRequest(State state)
        {
            foreach(var balance in state.Balances)
            {
                Console.WriteLine($"User: {balance.Key}, Balance: {balance.Value}");
            }
        }

        private static void HandleListTransactionsRequest(State state)
        {
            Console.WriteLine($"Listing Transactions not approved by user {state.Username}");
            Console.WriteLine("Number,From,To,Timestamp");
            bool nonApprovedTxsExist = false;
            List<Transaction> userTransactions = new List<Transaction>();
            foreach(var transaction in state.Transactions)
            {
                if(transaction.To == state.Username && transaction.Approved == 0)
                {
                    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    userTransactions.Add(transaction);
                    nonApprovedTxsExist = true;
                }
            }

            foreach(var userTransaction in userTransactions)
            {
                if(!state.Transactions.Any(p=>p.ApprovalTransactionNumber == userTransaction.Number))
                {
                    Console.WriteLine(PromptConstants.FormulateTransactionOutputInline(userTransaction));
                }
            }

            if (!nonApprovedTxsExist)
            {
                Console.WriteLine("All transactions approved. No transactions listed");
            }
        }

        private static void HandleHelpRequest()
        {
            Console.WriteLine(PromptConstants.FormulatePromptDialog());
        }

        private static void HandleLogRequest(State state)
        {
            Console.WriteLine($"Full Log");
            foreach (string line in state.OutputLog)
            {
                Console.WriteLine($"- {line}");
            }
        }

        private static void HandleGetTransactionRequest(State state, string command)
        {
            try
            {
                int transactionNumber = int.Parse(command.Split(' ')[1]);
                Transaction transaction = state.Transactions.FirstOrDefault(p => p.Number == transactionNumber);
                if (transaction != null)
                {
                    Console.WriteLine(PromptConstants.FormulateTransactionOutput(transaction));
                }
                else
                {
                    Console.WriteLine($"Transaction {transactionNumber} was not found.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to extract transaction number from command. Please ensure that there are no spaces after the transaction number and the command is written in a correct format");
                Console.WriteLine(e);
            }
        }

        private static void HandleHighTransactionRequest(State state)
        {
            int transactionNumber = state.Transactions.Count == 0 ? 0 : state.Transactions.Max(p => p.Number);
            Console.WriteLine($"Max Transaction Number {transactionNumber}");
        }

        private static void HandleInvalidRequest()
        {
            Console.WriteLine($"Invalid Request");
            Console.WriteLine(PromptConstants.FormulatePromptDialog());
        }

        public static Transaction GetApprovalTransaction(State state, string transactionNumber)
        {
            int txNumber = int.Parse(transactionNumber);
            var transaction = state.Transactions.FirstOrDefault(p => p.Number == txNumber);
            if(transaction == null)
            {
                Console.WriteLine($"Error getting transaction number {transactionNumber}. Transaction does not exist");
            }
            else if(transaction.To != state.Username || transaction.Approved == 1)
            {
                Console.WriteLine($"Error getting transaction number {transactionNumber}");
                Console.WriteLine($"The transaction have either been already approved or belongs to another user than the current user");
            }
            return transaction;
        }

        public static State HandleNewApprovalTransaction(State state, List<NodeDetails> seedNodes, Transaction transaction)
        {
            int transactionNumber = state.Transactions.Max(p => p.Number) + 1;
            if (state.Balances[state.Username] <= 0)
            {
                Console.WriteLine($"Not enough balance for user {state.Username}.");
                return state;
            }
            Transaction approvalTransaction = new Transaction(transactionNumber, "00", transaction.To, (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds, 1, transaction.Number);
            Transaction referenceTransaction = state.Transactions.First(p => p.Number == approvalTransaction.ApprovalTransactionNumber);

            state.Transactions.Add(approvalTransaction);
            state.Balances[transaction.To] += 1;
            state.Balances[referenceTransaction.From] -= 1;
            HandleTransactionRequest(seedNodes, approvalTransaction);
            return state;
        }

        public static State HandleNewApprovedTransaction(State state, List<NodeDetails> seedNodes, Transaction transaction)
        {
            int transactionNumber = state.Transactions.Max(p => p.Number) + 1;
            if (state.Balances[state.Username] <= 0)
            {
                Console.WriteLine($"Not enough balance for user {state.Username}.");
                return state;
            }
            Transaction approvalTransaction = new Transaction(transactionNumber, state.Username, transaction.To, (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds, 1, transaction.Number);

            state.Transactions.Add(approvalTransaction);
            state.Balances[approvalTransaction.To] += 1;
            state.Balances[approvalTransaction.From] -= 1;
            HandleTransactionRequest(seedNodes, approvalTransaction);
            return state;
        }

        private static void HandleTransactionRequest(List<NodeDetails> seedNodes, Transaction transaction)
        {
            foreach (var node in seedNodes)
            {
                byte[] message = MessageProcessor.ProcessMessage(BlockchainCommands.NEW_TRANS, new string[]
                {
                    transaction.Number.ToString(), transaction.From, transaction.To, transaction.Timestamp.ToString(),
                    transaction.Approved.ToString(), transaction.ApprovalTransactionNumber.ToString()
                });
                NodeBroadcasting.BroadcastToSeedNode(message, node);
            }

            string transactionOutput = PromptConstants.FormulateTransactionOutput(transaction);
            Console.WriteLine($"Transaction Added\n{transactionOutput}");
        }

        public static State HandleNewTransactionRequest(State state, List<NodeDetails> seedNodes, string to, int approved)
        {
            int transactionNumber = state.Transactions.Max(p => p.Number) + 1;
            if (!state.Balances.ContainsKey(to))
            {
                Console.WriteLine($"User {to} does not exist.");
                return state;
            }

            Transaction transaction = new Transaction(transactionNumber, state.Username, to, (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds, 0, 0);
            if (approved == 1)
            {
                transaction.Approved = 1;
                transaction.ApprovalTransactionNumber = transaction.Number;
                return HandleNewApprovedTransaction(state, seedNodes, transaction);
            }
            state.Transactions.Add(transaction);
            HandleTransactionRequest(seedNodes, transaction);
            return state;
        }
    }
}
