using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleIO
{
    /// <summary>
    /// 対話式のコンソール
    /// </summary>
    public class InteractiveConsole
    {
        readonly Encoding sjisEnc;

        /// <summary>
        /// 各行に何文字(半角で)分の文字があるかを保存する
        /// </summary>
        private Stack<int> rowLengthStack;

        public InteractiveConsole()
        {
            rowLengthStack = new Stack<int>();

            // shift-jisを使用可能にするためのやつ
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            sjisEnc = Encoding.GetEncoding("Shift_JIS");
        }

        /// <summary>
        /// テキストを画面に表示する
        /// </summary>
        /// <remarks>
        /// 入力されたテキストの総文字幅(コンソール上での)をスタックしている
        /// </remarks>
        /// <param name="txt">表示するテキスト</param>
        public void WriteLine(string txt)
        {
            StackRow(txt);
            Console.WriteLine(txt);
        }

        public string ReadLine()
        {
            var txt = Console.ReadLine();
            StackRow(txt);
            return txt;
        }

        public void WaitAnyKey()
        {
            Console.ReadKey(true);
        }
        
        /// <summary>
        /// 最終行を削除する
        /// </summary>
        public void ClearLastRow()
        {
            if(rowLengthStack.Count == 0) return;

            var len = rowLengthStack.Pop();

            // カーソルが一番左にあった時は新しい行だと考え、行を1つ戻す
            if(Console.CursorLeft == 0) Console.SetCursorPosition(0, Console.CursorTop - 1);

            ClearRow(len);
        }

        /// <summary>
        /// 行を保存する(何か入出力したときは必ずこれを通す)
        /// </summary>
        /// <param name="txt">文字</param>
        private void StackRow(string txt)
        {
            // shift-jisのバイト数が日本語だと2バイトなのでそれを使って文字の幅を算出している
            var txtLen = sjisEnc.GetByteCount(txt);
            rowLengthStack.Push(txtLen);
        }

        /// <summary>
        /// 行の文字を削除する(コンソール上で複数行になった場合それらも含めて)
        /// </summary>
        /// <param name="len">文字数</param>
        private void ClearRow(int len)
        {
            while(len > 0)
            {
                var w = Console.BufferWidth;
                Console.CursorLeft = 0;
                Console.Write(new string(' ', w));
                len -= w;
                Console.SetCursorPosition(0, Console.CursorTop - (len > 0 ? 1 : 0));
            }
        }

        public void PageClear()
        {
            Console.Clear();
        }

    }
}
