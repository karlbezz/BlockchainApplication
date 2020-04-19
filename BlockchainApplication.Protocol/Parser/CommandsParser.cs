﻿using BlockchainApplication.Data.Extensions;
using BlockchainApplication.Protocol.Commands;
using System;
using System.Text;

namespace BlockchainApplication.Protocol.Parser
{
    public static class CommandsParser
    {
        public static Command ParseCommand(byte[] message)
        {
            byte prefix = message[0];
            byte commandLength = message[1];
            byte[] command = new byte[commandLength];
            Array.Copy(message, 2, command, 0, commandLength);
            byte suffix = message[message.Length - 1];
            switch (prefix)
            {
                case 0x6e:
                    return ParseNewTransactionCommand(command);
                case 0x68:
                    return ParseHighestTransactionCommand(command);
                case 0x6d:
                    return ParseHighestTransactionResultCommand(command);
                case 0x67:
                    return ParseGetTransactionCommand(command);
                case 0x6f:
                    return ParseOkMessageCommand(command);
                case 0x66:
                    return ParseNotOkMessageCommand(command);
                default:
                    throw new Exception($"{DateTime.Now} - Invalid command passed.");                
            }
        }

        public static NewTransactionCommand ParseNewTransactionCommand(byte[] command)
        {
            byte[] transactionNumberBytes = command.GetRange(1, 2);
            byte[] fromUserBytes = command.GetRange(3, 2);
            byte[] toUserBytes = command.GetRange(5, 2);
            byte[] timestampBytes = command.GetRange(7, command.Length - 8);
            return new NewTransactionCommand()
            {
                Prefix = Encoding.ASCII.GetString(new byte[] { command[0] }),
                CommandType = BlockchainCommands.NEW_TRANS,
                TransactionNumber = transactionNumberBytes.ToInt(),
                FromUser = fromUserBytes.ToStringValue(),
                ToUser = fromUserBytes.ToStringValue(),
                Timestamp = timestampBytes.ToLong()
            };
        }

        public static HighestTransactionResultCommand ParseHighestTransactionResultCommand(byte[] command)
        {
            byte[] transactionNumberBytes = command.GetRange(1, 2);
            return new HighestTransactionResultCommand()
            {
                Prefix = Encoding.ASCII.GetString(new byte[] { command[0] }),
                CommandType = BlockchainCommands.HIGHEST_TRN_RES,
                TransactionNumber = transactionNumberBytes.ToInt()
            };
        }

        public static Command ParseHighestTransactionCommand(byte[] command)
        {
            return new Command()
            {
                Prefix = Encoding.ASCII.GetString(new byte[] { command[0] }),
                CommandType = BlockchainCommands.HIGHEST_TRN
            };
        }

        public static GetTransactionCommand ParseGetTransactionCommand(byte[] command)
        {
            byte[] transactionNumberBytes = command.GetRange(1, 2);
            return new GetTransactionCommand()
            {
                Prefix = Encoding.ASCII.GetString(new byte[] { command[0] }),
                TransactionNumber = transactionNumberBytes.ToInt(),
                CommandType = BlockchainCommands.GET_TRANS
            };
        }

        public static Command ParseOkMessageCommand(byte[] command)
        {
            return new Command()
            {
                Prefix = Encoding.ASCII.GetString(new byte[] { command[0] }),
                CommandType = BlockchainCommands.OK_MSG
            };
        }

        public static Command ParseNotOkMessageCommand(byte[] command)
        {
            return new Command()
            {
                Prefix = Encoding.ASCII.GetString(new byte[] { command[0] }),
                CommandType = BlockchainCommands.NOK_MSG
            };
        }
    }
}