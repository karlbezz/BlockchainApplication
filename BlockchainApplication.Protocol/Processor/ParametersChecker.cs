using BlockchainApplication.Data.Extensions;
using BlockchainApplication.Protocol.Commands;

namespace BlockchainApplication.Protocol.Processor
{
    public static class ParametersChecker
    {
        public static bool CheckParameters(BlockchainCommands commandType, params string[] messageParams)
        {
            if (commandType.Equals(BlockchainCommands.NEW_TRANS))
            {
                CheckNewTransactionParameters(commandType, messageParams);
            }
            else if (commandType.Equals(BlockchainCommands.GET_TRANS))
            {
                CheckGetTransactionParameters(commandType, messageParams);
            }

            return true;
        }

        private static bool CheckNewTransactionParameters(BlockchainCommands commandType, params string[] messageParams)
        {
            int transactionNumber = messageParams[0].ToInteger();
            byte[] transactionNumberBytes = transactionNumber.ToBytes();
            return (messageParams.Length >= 3 && transactionNumberBytes.Length <= 2);
        }

        private static bool CheckGetTransactionParameters(BlockchainCommands commandType, params string[] messageParams)
        {
            int transactionNumber = messageParams[0].ToInteger();
            byte[] transactionNumberBytes = transactionNumber.ToBytes();
            return transactionNumberBytes.Length <= 2;
        }
    }
}
