
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLRConverter
{

    public class TableSlider
    {
        private readonly Lexer _lexer = new("lexer.txt");
        public void RunSlider(Table table)
        {
            if (table == null) return;

            Stack<int> stack = new();
            int currRowNumber = 0;
            string currToken = _lexer.GetNextToken();

            Row currRow = table.Rows[currRowNumber];


            Stack<string> tempTokens = [];

            stack.Push(currRowNumber);

            while (true)
            {
                //условия конца разбора
                if (_lexer.IsEnd() && tempTokens.Count <= 1 && currRowNumber <= 1 && stack.Count <= 1 && currToken == table.RootName) return;

                //елси в данной строке есть что-то по данному токену, то начинаем разбор по строке
                if (currRow.Cells.TryGetValue(currToken, out TableCell cell))
                {
                    //если сдвиг
                    if (cell.Shift)
                    {
                        //переход на указанную строку
                        currRowNumber = cell.Number;
                        stack.Push(currRowNumber);

                        //сдвиг по входной цепочке
                        if (tempTokens.Count > 0)
                        {
                            currToken = tempTokens.Pop();
                        }
                        else
                        {
                            currToken = _lexer.GetNextToken();
                        }
                    }
                    //иначе свертка
                    else
                    {
                        //добавляем текущий токен в начало входно цепочки
                        //и сохраняем токен полученный от свертки
                        tempTokens.Push(currToken);
                        currToken = table.GrammarRules[cell.Number].Token;

                        //убираем из стека количество элементов, равное количеству грамматических вхождений в правиле
                        for (int i = 0; i < table.GrammarRules[cell.Number].SymbolsChain.Count; i++)
                        {
                            stack.Pop();
                        }
                        stack.TryPeek(out currRowNumber);
                    }
                }
                //иначе ошибка
                else
                {
                    throw new Exception($"given token: {currToken}, expected: " + string.Join(", ", currRow.Cells.Keys));
                }

                currRow = table.Rows[currRowNumber];
            }
        }
    }

}