using System;
using ConsoleController.IO;

namespace ConsoleIOTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var console = new ConsoleIO();
            Console.WriteLine(console.ReadLine());
        }
    }
}
