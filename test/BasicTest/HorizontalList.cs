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
            var select = console.HorizontalSelect(list, 12);
            console.WriteLine($"index:{select} value:{list[select]}");
            return true;
        }

        public bool MultiTest()
        {
            var console = new InteractiveConsole();
            var list = Enumerable.Range(0, 100).Select(x => x.ToString()).ToArray();
            var selectMultiValue = console.HorizontalMultiSelect(list, 10);
            foreach(var item in selectMultiValue)
            {
                console.WriteLine($"index:{item} value:{list[item]}");
            }
            return true;
        }

        public bool MultiTestWithDefault()
        {
            var console = new InteractiveConsole();
            var list = Enumerable.Range(0, 100).Select(x => x.ToString()).ToArray();
            var defaultSelect = new int[]{10, 20, 35, 41};
            var selectMultiValue = console.HorizontalMultiSelect(list, 10, defaultSelect);
            foreach(var item in selectMultiValue)
            {
                console.WriteLine($"index:{item} value:{list[item]}");
            }
            return true;
        }
    }
}