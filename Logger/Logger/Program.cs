using System;

namespace Logger
{
    class Program
    {
        static void Main(string[] args)
        {
            Token classInstance = new Token();
            classInstance.GetTokens();
            classInstance.SendTokens();
            Console.ReadLine();
        }
    }
}
