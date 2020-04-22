using BlockchainApplication.Data.Extensions;
using BlockchainApplication.Protocol.Commands;
using System;
using System.Collections.Generic;

namespace BlockchainApplication.Protocol.Processor
{
    public static class MessageProcessor
    {
        public static byte[] ProcessMessage(BlockchainCommands commandType, params string[] messageParams)
        {
            bool paramsValid = ParametersChecker.CheckParameters(commandType, messageParams);
            if (paramsValid)
            {
                byte[] command = CommandProcessor.ProcessCommand(commandType, messageParams);
                byte commandLength = command.Length.ToByte();
                List<byte> message = new List<byte>() { 0x32, commandLength };
                message.AddRange(command);
                message.Add(0x33);
                return message.ToArray();
            }
            else
            {
                throw new Exception($"{DateTime.Now} - Invalid Message Parameters passed for command {nameof(commandType)}");
            }
        }
    }
}
    