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
            builder.AppendLine($"- getTx [txNum] to get the transaction");
            builder.AppendLine($"- newTx [to] to add a new transaction");
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
            return builder.ToString();
        }
    }
}
