using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Objects
{
    public class Transaction
    {
        public string Hash { get; set; }
        public int Number { get; set; }
        public int Timestamp { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int Amount { get; set; }
        public int Approved { get; set; }
        public int ApprovalTransactionNumber { get; set; }

        public Transaction(int number, string from, string to, int timestamp, int amount = 1)   
        {
            this.Number = number;
            this.From = from;
            this.To = to;
            this.Timestamp = timestamp;
            this.Amount = amount;
            this.Approved = 1;
            this.ApprovalTransactionNumber = 0;
        }

        public Transaction(int number, string from, string to, int timestamp, int approved, int approvalTn, int amount = 1)
        {
            this.Number = number;
            this.From = from;
            this.To = to;
            this.Timestamp = timestamp;
            this.Amount = amount;
            this.Approved = approved;
            this.ApprovalTransactionNumber = approvalTn;
        }
    }
}
