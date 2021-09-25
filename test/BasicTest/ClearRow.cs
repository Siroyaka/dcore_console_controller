using ConsoleIO;

namespace ConsoleIOTest.BasicTest
{
    public class ClearRow
    {
        public bool SingleRow()
        {
            var console = new InteractiveConsole();
            console.WriteLine("カーソルが一番左にあった時は新しい行だと考え、行を1つ戻す");
            console.WriteLine("test2");
            console.ReadLine();
            console.ClearLastRow();
            console.ClearLastRow();
            console.ClearLastRow();
            console.WriteLine("delete OK");
            return true;
        }

        public bool Block()
        {
            var console = new InteractiveConsole();
            console.WriteLine("これは残す");
            console.WriteLine("test1", "test2", "test3", "test4", "test5");
            console.WaitAnyKey();
            console.ClearLastBlock();
            return true;
        }

    }
}