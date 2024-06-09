namespace SLRConverter
{
    public class GrammarRule(
        string token,
        List<string> symbolsChain,
        List<RowKey> directionSymbols
    )
    {
        // Левый нетерминал правила
        public string Token { get; set; } = token;

        // Цепочка символов, выводимая из нетерминала
        public List<string> SymbolsChain { get; set; } = symbolsChain;

        // Множество направляющих символов
        public List<RowKey> DirectionSymbols { get; set; } = directionSymbols;
    }

    //Ряд таблицы
    public class Row
    {
        // Ключ словаря - это символ по которому происходит либо сдвиг, либо свертка
        public Dictionary<string, TableCell> Cells { get; set; } = [];
    }

    // Ячейка таблицы
    public struct TableCell
    {
        // Сдвиг (true) или свертка (false)
        public bool Shift;

        // Номер ряда для свертки или правило свёртки
        public int Number;
    }

    // Грамматическое вхождение
    public struct RowKey(string token, int row, int column)
    {
        public string Token { get; set; } = token;

        public int Row { get; set; } = row;

        public int Column { get; set; } = column;
    }

    public class Table(List<Row> rows, List<GrammarRule> rules)
    {
        public string RootName { get; set; }
        public List<Row> Rows { get; set; } = rows;
        public List<GrammarRule> GrammarRules { get; set; } = rules;
        public List<List<RowKey>> RowNames { get; set; }
        public List<string> ColumnNames { get; set; } = [];
    }
}