using System;
using ConsoleIOTest.BasicTest;

namespace ConsoleIOTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var test = new ClearRow();
            //test.SingleRow();
            var pagerTest = new Pager();
            pagerTest.BaseTest();
            //var test = new HorizontalList();
            //test.BaseTest();
            
        }

        static void TestInput()
        {
            Console.Write("test\ntest\ntest\ntest\ntest\ntest\ntest\ntest\ntest\ntest\ntest\ntest\n");
            Console.ReadKey();
            var (a, b) = Console.GetCursorPosition();
            Console.SetCursorPosition(0, b - 7);
            Console.Write("    \n    \n    \n    \n    \n    \n    \n    \n");
        }
    }
}
