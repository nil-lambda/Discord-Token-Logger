using System;

namespace TokenLogger
{
    class Program
    {
        static void Main()
        {
            Engine engine = new Engine();
            engine.Run();

            Console.ReadKey();
        }
    }
}