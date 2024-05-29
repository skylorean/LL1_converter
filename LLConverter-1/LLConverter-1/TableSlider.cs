namespace LLConverter_1
{
    public class TableSlider
    {
        private readonly Lexer _lexer;

        public TableSlider(string lexerFileName)
        {
            _lexer = new Lexer(lexerFileName);
        }

        public void RunSlider(Table table)
        {
            if (table == null) return;

            Stack<int> stack = new();
            int currRowNumber = 0;
            string currToken = _lexer.GetNextToken();
            Row currRow = table.Rows[currRowNumber];

            // Выход из цикла только, если строка - последняя И на вход нет лексем И стэк пуст
            while (true)
            {
                if (currRow.End && stack.Count == 0 && _lexer.IsEnd()) break;

                // Если в направляющих символах строки таблички есть наш токен
                if (currRow.DirectionSymbols.Contains(currToken))
                {
                    // Стэк
                    if (currRow.MoveToNextLine) stack.Push(currRowNumber + 1);
                    // Указатель на другую строку
                    if (currRow.Pointer.HasValue)
                    {
                        currRowNumber = currRow.Pointer.Value;
                    }
                    else
                    {
                        if (!stack.TryPop(out currRowNumber))
                        {
                            throw new Exception($"currRowNumber: {currRowNumber}, currToken: {currToken}, stack is empty");
                        }

                    }

                    if (currRow.Shift) currToken = _lexer.GetNextToken();
                }
                else if (!currRow.Error)
                {
                    currRowNumber++;
                }
                else
                {
                    throw new Exception($"currRowNumber: {currRowNumber}, currToken: {currToken}, directionSymbols not contains currToken");
                }

                if (currRow.End && stack.Count == 0 && _lexer.IsEnd()) break;

                currRow = table.Rows[currRowNumber];
            }
        }
    }

}