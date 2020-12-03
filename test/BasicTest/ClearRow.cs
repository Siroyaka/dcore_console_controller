using ConsoleIO;

namespace ConsoleIOTest.BasicTest
{
    public class ClearRow
    {
        public bool Test()
        {
            var console = new InteractiveConsole();
            console.WriteLine("test1");
            console.WriteLine("test2");
            console.ReadLine();
            console.ClearLastRow();
            console.ClearLastRow();
            console.ClearLastRow();
            console.WriteLine("delete OK");
            return true;
        }
    }
}