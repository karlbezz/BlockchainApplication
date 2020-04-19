using BlockchainApplication.Data.Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlockchainApplication.Data
{
    public static class FileHandling
    {
        public static List<NodeDetails> LoadSeedNodesFromCsv(string path, string ip, int port)
        {
            List<NodeDetails> nodeDetails = new List<NodeDetails>();
            if (!File.Exists(path))
            {
                throw new Exception("File path holding node details does not exist");
            }

            StreamReader reader = new StreamReader(path);
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string[] nodeDetailsSplit = reader.ReadLine().Split(',');
                NodeDetails details = new NodeDetails()
                {
                    IP = nodeDetailsSplit[0],
                    Port = int.Parse(nodeDetailsSplit[1])
                };

                if (details.IP != ip || details.Port != port)
                {
                    nodeDetails.Add(details);
                }
            }

            return nodeDetails;
        }
    }
}
