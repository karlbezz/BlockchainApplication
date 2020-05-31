using BlockchainApplication.Data.Objects;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Utilities
{
    public static class Hashing
    {
        public static string GenerateHash(string data)
        {
            SHA256 digest = SHA256.Create();
            return Hex.ToHexString(digest.ComputeHash(Encoding.UTF8.GetBytes(data)));
        }
        public static Block GenerateBlockHash(Block block, int treshold)
        {
            int nonce = 0;
            string comparator = "";
            for (int i = 0; i < treshold; i++)
            {
                comparator += "0";
            }

            while (true)
            {
                block.HashedContent.Nonce = nonce;
                string hash = GenerateHash(JsonConvert.SerializeObject(block.HashedContent));
                if (hash.StartsWith(comparator))
                {
                    block.Hash = hash;
                    return block;
                }

                nonce++;
            }
        }
    }
}
