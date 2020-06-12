using BlockchainApplication.SBB.Protocol.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.SBB.Protocol.Parser
{
    public static class MessageParser
    {
        public static Command ParseMessage(byte[] messageBytes)
        {
            List<byte> bytesList = messageBytes.ToList();
            ushort length = BitConverter.ToUInt16(bytesList.GetRange(1, 2).ToArray(), 0);
            CommandType identifier = (CommandType)Convert.ToChar(bytesList[3]);
            byte[] asciiCommand = bytesList.GetRange(4, length - 1).ToArray();
            string jsonCommand = Encoding.ASCII.GetString(asciiCommand);
            var command = ParseCommand(identifier, jsonCommand);
            command.CommandType = identifier;
            return command;
        }

        private static Command ParseCommand(CommandType commandType, string jsonCommand)
        {
            switch (commandType)
            {
                case CommandType.BLOCK:
                    return ParseBlockCommand(jsonCommand);
                case CommandType.BLOCK_HASHES:
                    return ParseBlockHashesCommand(jsonCommand);
                case CommandType.COUNT:
                    return ParseCountCommand(jsonCommand);
                case CommandType.GET_BLOCK_HASHES:
                    return ParseGetBlockHashesCommand(jsonCommand);
                case CommandType.GET_COUNT:
                    return ParseGetCountCommand(jsonCommand);
                case CommandType.NEW_BLOCK:
                    return ParseNewBlockCommand(jsonCommand);
                case CommandType.REQ_BLOCK:
                    return ParseRequestBlockCommand(jsonCommand);
                default:
                    throw new Exception("Invalid command");
            }
        }

        private static BlockCommand ParseBlockCommand(string jsonCommand)
        {
            return JsonConvert.DeserializeObject<BlockCommand>(jsonCommand);
        }

        private static BlockHashesCommand ParseBlockHashesCommand(string jsonCommand)
        {
            return JsonConvert.DeserializeObject<BlockHashesCommand>(jsonCommand);
        }

        private static CountCommand ParseCountCommand(string jsonCommand)
        {
            return JsonConvert.DeserializeObject<CountCommand>(jsonCommand);
        }

        private static GetBlockHashesCommand ParseGetBlockHashesCommand(string jsonCommand)
        {
            return JsonConvert.DeserializeObject<GetBlockHashesCommand>(jsonCommand);
        }

        private static GetCountCommand ParseGetCountCommand(string jsonCommand)
        {
            return JsonConvert.DeserializeObject<GetCountCommand>(jsonCommand);
        }

        private static NewBlockCommand ParseNewBlockCommand(string jsonCommand)
        {
            return JsonConvert.DeserializeObject<NewBlockCommand>(jsonCommand);
        }

        private static RequestBlockCommand ParseRequestBlockCommand(string jsonCommand)
        {
            return JsonConvert.DeserializeObject<RequestBlockCommand>(jsonCommand);
        }
    }
}
