using BlockchainApplication.Data.Extensions;
using BlockchainApplication.Data.Utilities;
using BlockchainApplication.Protocol.Commands;
using System;
using System.Collections.Generic;

namespace BlockchainApplication.Protocol.Processor
{
    public static class CommandProcessor
    {
        public static byte[] ProcessCommand(BlockchainCommands commandType, params string[] messageParams)
        {
            switch (commandType)
            {
                case BlockchainCommands.NEW_TRANS:
                    if(messageParams.Length == 3)
                    {
                        return ProcessNewTransactionCommand(messageParams[0], messageParams[1], messageParams[2]);
                    }
                    return ProcessNewTransactionCommand(messageParams[0], messageParams[1], messageParams[2], messageParams[3]);
                case BlockchainCommands.HIGHEST_TRN:
                    return ProcessHighestTransactionCommand();
                case BlockchainCommands.HIGHEST_TRN_RES:
                    return ProcessHighestTransactionResultCommand(messageParams[0]);
                case BlockchainCommands.GET_TRANS:
                    return ProcessGetTransactionCommand(messageParams[0]);
                case BlockchainCommands.OK_MSG:
                    return ProcessOkMessageCommand();
                case BlockchainCommands.NOK_MSG:
                    return ProcessNotOkMessageCommand();
                default:
                    throw new Exception($"{DateTime.Now} - Invalid Command {nameof(commandType)}");
            }
        }

        private static byte[] ProcessNewTransactionCommand(string transactionNumber, string fromUser, string toUser)
        {
            byte[] prefixBytes = "n".ToBytes();
            byte[] tnBytes = transactionNumber.ToInteger().ToBytes();
            byte[] fromUserBytes = fromUser.ToBytes();
            byte[] toUserBytes = toUser.ToBytes();
            byte[] timestamp = DateTimeUtilities.ConvertToUnixTimestampBytes(DateTime.Now);

            List<byte> transactionCommandBytes = new List<byte>();
            transactionCommandBytes.AddRange(prefixBytes);
            transactionCommandBytes.AddRange(tnBytes);
            transactionCommandBytes.AddRange(fromUserBytes);
            transactionCommandBytes.AddRange(toUserBytes);
            transactionCommandBytes.AddRange(timestamp);
            return transactionCommandBytes.ToArray();
        }

        private static byte[] ProcessNewTransactionCommand(string transactionNumber, string fromUser, string toUser, string timestamp)
        {
            byte[] prefixBytes = "n".ToBytes();
            byte[] tnBytes = transactionNumber.ToInteger().ToBytes();
            byte[] fromUserBytes = fromUser.ToBytes();
            byte[] toUserBytes = toUser.ToBytes();
            byte[] timestampBytes = timestamp.ToBytes();

            List<byte> transactionCommandBytes = new List<byte>();
            transactionCommandBytes.AddRange(prefixBytes);
            transactionCommandBytes.AddRange(tnBytes);
            transactionCommandBytes.AddRange(fromUserBytes);
            transactionCommandBytes.AddRange(toUserBytes);
            transactionCommandBytes.AddRange(timestampBytes);
            return transactionCommandBytes.ToArray();
        }

        private static byte[] ProcessHighestTransactionCommand()
        {
            byte[] prefixBytes = "h".ToBytes();
            return prefixBytes;
        }

        private static byte[] ProcessHighestTransactionResultCommand(string transactionNumber)
        {
            byte[] prefixBytes = "m".ToBytes();
            byte[] tnBytes = transactionNumber.ToInteger().ToBytes();
            List<byte> highestTransactionCommandBytes = new List<byte>();
            highestTransactionCommandBytes.AddRange(prefixBytes);
            highestTransactionCommandBytes.AddRange(tnBytes);
            return highestTransactionCommandBytes.ToArray();
        }

        private static byte[] ProcessGetTransactionCommand(string transactionNumber)
        {
            byte[] prefixBytes = "g".ToBytes();
            byte[] tnBytes = transactionNumber.ToInteger().ToBytes();
            List<byte> getTransactionCommandBytes = new List<byte>();
            getTransactionCommandBytes.AddRange(prefixBytes);
            getTransactionCommandBytes.AddRange(tnBytes);
            return getTransactionCommandBytes.ToArray();
        }

        private static byte[] ProcessOkMessageCommand()
        {
            byte[] prefixBytes = "o".ToBytes();
            return prefixBytes;
        }

        private static byte[] ProcessNotOkMessageCommand()
        {
            byte[] prefixBytes = "f".ToBytes();
            return prefixBytes;
        }
    }
}
