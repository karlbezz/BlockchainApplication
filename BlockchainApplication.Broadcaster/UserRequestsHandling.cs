using BlockchainApplication.Data.Constants;
using BlockchainApplication.Data.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            else
            {
                HandleInvalidRequest();
            }
        }

        public static void HandleHelpRequest()
        {
            Console.WriteLine(PromptConstants.FormulatePromptDialog());
        }

        public static void HandleLogRequest(State state)
        {
            Console.WriteLine($"Full Log");
            foreach (string line in state.OutputLog)
            {
                Console.WriteLine($"- {line}");
            }
        }

        public static void HandleGetTransactionRequest(State state, string command)
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

        public static void HandleHighTransactionRequest(State state)
        {
            int transactionNumber = state.Transactions.Max(p => p.Number);
            Console.WriteLine($"Max Transaction Number {transactionNumber}");
        }

        public static void HandleInvalidRequest()
        {
            Console.WriteLine($"Invalid command passed.");
            Console.WriteLine(PromptConstants.FormulatePromptDialog());
        }
    }
}
