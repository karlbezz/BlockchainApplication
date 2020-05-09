using System;

namespace BlockchainApplication.Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            string code = args[0];
            if(args.Length == 0 || args[0].Length == 0)
            {
                Console.WriteLine("Invalid arguments");
            }

            string[] codeValues = code.Trim().Split(',');
            int xValue = 0;
            for(int i = 0; i < codeValues.Length; i++)
            {
                int codeValue = Convert.ToInt32(codeValues[i], 16);
                switch (codeValue)
                {
                    case (int)OPCodes.OUTPUT:
                        IntepretOutput(xValue);
                        break;
                    case (int)OPCodes.INPUT:
                        xValue = InterpretInput();
                        break;
                    case (int)OPCodes.INC:
                        xValue = InterpretIncrement(xValue);
                        break;
                    default:
                        throw new Exception("Invalid Arguments");
                }
            }
            Console.WriteLine($"Final xValue {xValue}");
        }

        static void IntepretOutput(int xValue)
        {
            Console.WriteLine(xValue);
        }

        static int InterpretInput()
        {
            int xValue = 0;
            Console.WriteLine("Input a number");
            string input = Console.ReadLine();
            while (!int.TryParse(input, out xValue))
            {
                Console.WriteLine("Invalid input please try again");
                input = Console.ReadLine();
            }

            return xValue;
        }

        static int InterpretIncrement(int xValue)
        {
            xValue++;
            return xValue;
        }
    }
}
