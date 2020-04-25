using BlockchainApplication.Data.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Constants
{
    public static class PromptConstants
    {
        public static string FormulatePromptDialog()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Available commands..");
            builder.AppendLine($"- help to popup this dialog");
            builder.AppendLine($"- log to show output log");
            builder.AppendLine($"- highTx to get highest transaction");
            builder.AppendLine($"- listTx to list all the transactions that have not yet been approved");
            builder.AppendLine($"- balance to show user\'s balance");
            builder.AppendLine($"- balances to show all users balances");
            builder.AppendLine($"- getTx [txNum] to get the transaction");
            builder.AppendLine($"- newTx [to] to add a new transaction");
            builder.AppendLine($"- approveTx [txNum] to approve a transaction");
            return builder.ToString();
        }

        public static string FormulateTransactionOutput(Transaction transaction)
        {
            StringBuilder builder = new StringBuilder();
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(transaction.Timestamp).ToLocalTime();
            builder.AppendLine($"Transaction {transaction.Number}");
            builder.AppendLine($"- From: {transaction.From}");
            builder.AppendLine($"- To: {transaction.To}");
            builder.AppendLine($"- Timestamp: {dtDateTime}");
            builder.AppendLine($"- Approved: {transaction.Approved == 1}");
            builder.AppendLine($"- Approved Transaction: {transaction.ApprovalTransactionNumber}");
            return builder.ToString();
        }

        public static string FormulateTransactionOutputInline(Transaction transaction)
        {
            StringBuilder builder = new StringBuilder();
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(transaction.Timestamp).ToLocalTime();
            builder.Append($"{transaction.Number},{transaction.From},{transaction.To},{dtDateTime}");
            return builder.ToString();
        }
    }
}
