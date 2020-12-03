using ConsoleIO;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleIOTest.BasicTest
{
    public class HorizontalList
    {
        public bool BaseTest()
        {
            var console = new InteractiveConsole();
            var list = Enumerable.Range(0, 100).Select(x => x.ToString()).ToArray();
            var select = console.HorizontalMultiSelect(list, 12);
            console.WriteLine($"index:{select} value:{list[select]}");
            return true;
        }
    }
}