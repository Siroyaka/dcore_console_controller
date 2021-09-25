using System;
using System.Collections.Generic;
using System.Linq;
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
        /// 各行に何文字(半角で)分の文字があるかをもつ
        /// </summary>
        private Stack<int> rowLengthStack;

        /// <summary>
        /// 行のまとまりをもつ
        /// </summary>
        private Stack<int> rowBlockStack;

        private int stackBlockCount = 0;
        private bool blockStackMode = false;

        public InteractiveConsole()
        {
            rowLengthStack = new Stack<int>();

            rowBlockStack = new Stack<int>();

            // shift-jisを使用可能にするためのやつ
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            sjisEnc = Encoding.GetEncoding("Shift_JIS");
        }

        /// <summary>
        /// テキストを画面に表示する(複数行可能)
        /// </summary>
        /// <remarks>
        /// 入力されたテキストの総文字幅(コンソール上での)をスタックしている
        /// </remarks>
        /// <param name="txts">表示するテキスト(複数可能)</param>
        public void WriteLine(params string[] txts)
        {
            Console.CursorVisible = false;
            if(blockStackMode)
            {
                stackBlockCount += txts.Length;
            }
            else
            {
                rowBlockStack.Push(txts.Length);
            }
            var outputLine = string.Join('\n', txts);

            foreach(var txt in txts)
            {
                StackRow(txt);
            }
            Console.WriteLine(outputLine);
            Console.CursorVisible = true;
        }

        public void StartStackBlock()
        {
            blockStackMode = true;
        }

        public void EndStackBlock()
        {
            blockStackMode = false;
            rowBlockStack.Push(this.stackBlockCount);
            this.stackBlockCount = 0;
        }

        public string ReadLine()
        {
            var txt = Console.ReadLine();
            StackRow(txt);
            rowBlockStack.Push(1);
            return txt;
        }

        public void WaitAnyKey()
        {
            Console.ReadKey(true);
        }

        public void ClearLastBlock()
        {
            if(rowLengthStack.Count == 0) return;
            if(rowBlockStack.Count == 0) return;

            // Calculate the number of rows to delete
            // カーソルが一番左にあった時は新しい行だと考え、行を1つ戻す
            var blockRows = rowBlockStack.Pop();
            int endCursorTop = Console.CursorTop - (Console.CursorLeft == 0 ? 1 : 0);
            var p = rowLengthStack.Pop();
            rowLengthStack.Push(p);
            var rowCount = (int)Enumerable.Range(0, blockRows).Sum(_ => Math.Ceiling((double)rowLengthStack.Pop() / Console.BufferWidth));
            int startCursorTop = rowCount > endCursorTop ? 0 : (endCursorTop - rowCount + 1);
            var blankTexts = string.Join('\n', Enumerable.Range(0, rowCount).Select(_ => new string(' ', Console.BufferWidth)).ToArray());
            SetCursorPosition(0, startCursorTop);
            Console.WriteLine(blankTexts);
            SetCursorPosition(0, startCursorTop);

            //for(var blockLen = rowBlockStack.Pop(); blockLen > 0; blockLen--)
            //{
            //    var len = rowLengthStack.Pop();

            //    // カーソルが一番左にあった時は新しい行だと考え、行を1つ戻す
            //    if(Console.CursorLeft == 0) SetCursorPosition(0, Console.CursorTop - 1);
            //    ClearRow(len);
            //    if(Console.CursorTop == 0) break;
            //}
        }

        public void ClearAllRow()
        {
            if(rowLengthStack.Count == 0) return;

            while(rowLengthStack.TryPop(out int len))
            {
                // カーソルが一番左にあった時は新しい行だと考え、行を1つ戻す
                if(Console.CursorLeft == 0) SetCursorPosition(0, Console.CursorTop - 1);

                ClearRow(len);
                if(Console.CursorTop == 0) break;
            }

            rowLengthStack.Clear();
            rowBlockStack.Clear();
        }

        // 左右でページ切り替えを行う
        public int HorizontalSelect(string[] list, int viewNum, int defalutValue = 0)
        {
            var selectValue = defalutValue;
            var loop = true;
            while(loop)
            {
                int maxHeight = Math.Min(Console.BufferHeight - 1, viewNum);
                int pageCount = list.Length / maxHeight + (list.Length % maxHeight == 0 ? 0 : 1);
                int page = selectValue / maxHeight;

                Func<int, string> choiseIcon = (int i) => i == (selectValue % maxHeight) ? ">" : " ";

                var viewList = list.Skip(page * maxHeight).Take(maxHeight).Select((x, i) => $"{choiseIcon(i)} {x}").ToArray();

                WriteLine(viewList);

                var input = Console.ReadKey(true);

                var addValue = 0;

                switch(input.Key)
                {
                    case ConsoleKey.Enter:
                        loop = false;
                        break;
                    // 上への動作
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.K:
                        addValue = -1;
                        break;
                    // 下への動作
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.J:
                        addValue = 1;
                        break;
                    // 左への動作
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.H:
                        addValue = maxHeight * -1;
                        break;
                    // 右への動作
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.L:
                        addValue = maxHeight;
                        break;
                    default:
                        break;
                }
                ClearLastBlock();
                selectValue = Math.Max(Math.Min(selectValue + addValue, list.Length - 1), 0);
            }
            return selectValue;
        }

        public int[] HorizontalMultiSelect(string[] list, int viewNum)
        {
            return HorizontalMultiSelect(list, viewNum, new int[]{});
        }

        public int[] HorizontalMultiSelect(string[] list, int viewNum, int[] selectedNums)
        {
            bool[] selectedFlgs = list.Select((_, i) => selectedNums.Contains(i)).ToArray();
            int selectNum = 0;
            var loop = true;
            while(loop)
            {
                int maxHeight = Math.Min(Console.BufferHeight - 1, viewNum);
                int pageCount = list.Length / maxHeight + (list.Length % maxHeight == 0 ? 0 : 1);
                var page = selectNum / maxHeight;

                string choiseIcon(int i)
                {
                    var selected = i == selectNum ? ">" : " ";
                    var choosed = selectedFlgs[i] ? "*" : " ";
                    return $"{selected} {choosed}";
                }

                var viewList = list.Select((x, i) => $"{choiseIcon(i)} {x}").Skip(page * maxHeight).Take(maxHeight).ToArray();

                WriteLine(viewList);

                var input = Console.ReadKey(true);

                var addValue = 0;

                switch(input.Key)
                {
                    case ConsoleKey.Enter:
                        loop = false;
                        break;
                    // 上への動作
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.K:
                        addValue = -1;
                        break;
                    // 下への動作
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.J:
                        addValue = 1;
                        break;
                    // 左への動作
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.H:
                        addValue = maxHeight * -1;
                        break;
                    // 右への動作
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.L:
                        addValue = maxHeight;
                        break;
                    case ConsoleKey.Spacebar:
                        selectedFlgs[selectNum] = !selectedFlgs[selectNum];
                        break;
                    default:
                        break;
                }
                ClearLastBlock();
                selectNum = Math.Max(Math.Min(selectNum + addValue, list.Length - 1), 0);
            }
            return selectedFlgs.Select((x, i) => new {value = x, index = i}).Aggregate(new List<int>(), (acc, x) => {
                if(x.value) acc.Add(x.index);
                return acc;
            }).ToArray();
        }

        /// <summary>
        /// 与えられた関数で取得できるstringをコンソール上に表示する
        /// 左右でページ切り替えを行う
        /// indexはページ数より1すくないので注意してね
        /// </summary>
        /// <param name="readData">indexをもとにstringを出力する関数</param>
        /// <param name="maxIndex">ページ数の最大数</param>
        /// <returns>enterを押した時に表示していたページのindex</returns>
        public int ShowPager(Func<int, string[]> readData, int maxIndex, int defaultIndex = 0)
        {
            var loop = true;
            var index = defaultIndex;
            while(loop)
            {
                var text = readData(index);

                StartStackBlock();
                WriteLine($"----page:{index + 1}----");
                WriteLine(text);
                EndStackBlock();

                var input = Console.ReadKey(true);

                var addValue = 0;

                switch(input.Key)
                {
                    case ConsoleKey.Enter:
                        loop = false;
                        break;
                    // 上への動作
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.K:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.H:
                        addValue = -1;
                        break;
                    // 下への動作
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.J:
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.L:
                        addValue = 1;
                        break;
                    default:
                        break;
                }
                ClearLastBlock();

                index = Math.Max(Math.Min(index + addValue, maxIndex - 1), 0);
            }
            return index;
        }

        /// <summary>
        /// 最終行を削除する
        /// </summary>
        public void ClearLastRow()
        {
            if(rowLengthStack.Count == 0) return;
            if(rowBlockStack.Count == 0) return;

            var len = rowLengthStack.Pop();

            // カーソルが一番左にあった時は新しい行だと考え、行を1つ戻す
            if(Console.CursorLeft == 0) SetCursorPosition(0, Console.CursorTop - 1);

            ClearRow(len);

            // ブロック数の変更
            var block = rowBlockStack.Pop();
            if(block > 1) rowBlockStack.Push(block - 1);
        }

        /// <summary>
        /// カーソル位置を移動するが、バッファ外にいかないようにする
        /// </summary>
        private void SetCursorPosition(int left, int top)
        {
            // cursor left and top position do not under 0 and over buffer.
            int leftPosition = Math.Max(0, Math.Min(left, Console.BufferWidth));
            // if(left > Console.BufferWidth) left = Console.BufferWidth;
            // else if (left < 0) left = 0;

            int topPosition = Math.Max(0, Math.Min(top, Console.BufferHeight));
            // if(top > Console.BufferHeight) top = Console.BufferHeight;
            // else if (top < 0) top = 0;

            Console.SetCursorPosition(leftPosition, topPosition);
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
            Console.CursorVisible = false;
            while(len > 0)
            {
                var w = Console.BufferWidth;
                Console.CursorLeft = 0;
                Console.Write(new string(' ', w));
                len -= w;

                var cursorTop = Console.CursorTop;
                //SetCursorPosition(0, cursorTop - (len > 0 ? 1 : 0));
                //SetCursorPosition(0, cursorTop - (len > 0 ? 2 : 1));
                SetCursorPosition(0, cursorTop - 1);

                // カーソル位置が上限だったときは終わる
                if(cursorTop == 0) break;
            }
            Console.CursorVisible = true;
        }

        public void PageClear()
        {
            Console.Clear();
        }

    }
}
