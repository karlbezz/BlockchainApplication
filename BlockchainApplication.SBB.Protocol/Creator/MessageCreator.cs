using BlockchainApplication.SBB.Protocol.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.SBB.Protocol.Creator
{
    public static class MessageCreator
    {
        public static byte[] CreateMessage(Command command)
        {
            string serializedCommand = JsonConvert.SerializeObject(command);
            byte[] asciiCommand = Encoding.ASCII.GetBytes(serializedCommand);
            ushort length = Convert.ToUInt16(asciiCommand.Length + 1);
            byte[] lengthBytes = BitConverter.GetBytes(length);
            byte identifier = Convert.ToByte(command.CommandType);
            return AddToByteArray(lengthBytes, identifier, asciiCommand);
        }

        private static byte[] AddToByteArray(byte[] lengthBytes, byte identifier, byte[] asciiCommand, byte stx = 0x32, byte etx = 0x33)
        {
            List<byte> messageList = new List<byte>() { stx };
            messageList.AddRange(lengthBytes);
            messageList.Add(identifier);
            messageList.AddRange(asciiCommand);
            messageList.Add(etx);
            return messageList.ToArray();
        }
    }
}
