using ConsoleIO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ConsoleIOTest.BasicTest
{
    public class Pager
    {
        public bool BaseTest()
        {
            var console = new InteractiveConsole();
            string[] showData(int index)
            {
                var sb = new List<string>();
                sb.Add($"test {index} test");
                sb.Add("文章が表示されます");
                sb.Add("いろいろひょじ");
                return sb.ToArray();
            }
            var index = console.ShowPager(showData, 100);
            console.WriteLine(index.ToString());
            return true;
        }
    }
}