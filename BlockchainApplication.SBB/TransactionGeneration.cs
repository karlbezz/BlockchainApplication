using BlockchainApplication.Data.Objects;
using BlockchainApplication.Data.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainApplication.SBB
{
    public static class TransactionGeneration
    {
        public static SBBTransaction GenerateCoinbaseTransaction(string publicKey)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            SBBTransaction transaction = new SBBTransaction("0", publicKey);
            string jsonTransaction = JsonConvert.SerializeObject(transaction.HashedContent);
            transaction.Hash = Hashing.GenerateHash(jsonTransaction);
            return transaction;
        }

        public static SBBTransaction GenerateUserDefinedTransaction(string publicKey, string to)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            SBBTransaction transaction = new SBBTransaction(publicKey, to);
            string jsonTransaction = JsonConvert.SerializeObject(transaction.HashedContent);
            transaction.Hash = Hashing.GenerateHash(jsonTransaction);
            return transaction;
        }
    }
}
