namespace LLConverter_1
{
    public class GrammarRule(
        string token,
        List<string> symbolsChain,
        List<string> directionSymbols
    )
    {
        // левый нетерминал правила
        public string Token { get; set; } = token;

        // цепочка символов, которая выводится из нетерминала token
        public List<string> SymbolsChain { get; set; } = symbolsChain;

        // направляющее множество символов, с которых может начинаться правило
        public List<string> DirectionSymbols { get; set; } = directionSymbols;
    }

    public class Row(
        string token,
        List<string> directionSymbols,
        bool shift,
        bool error = true,
        int? pointer = null,
        bool moveToNextLine = false,
        bool end = false
    )
    {
        // нетерминал
        public string Token { get; set; } = token;

        // направляющее множество символов, с которых может начинаться правило
        public List<string> DirectionSymbols { get; set; } = directionSymbols;

        // сдвиг
        public bool Shift { get; set; } = shift;

        public bool Error { get; set; } = error;

        public int? Pointer { get; set; } = pointer;

        // переходить на следующую строку после разбора текущей строки или нет (заносить ли в стек)
        public bool MoveToNextLine { get; set; } = moveToNextLine;

        public bool End { get; set; } = end;
    }

    public class Table(List<Row> rows)
    {
        public List<Row> Rows { get; set; } = rows;
    }
}