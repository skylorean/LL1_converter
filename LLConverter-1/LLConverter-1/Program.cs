using LLConverter_1;

class Program
{
    public static void Main(string[] args)
    {
        FileParser fileParser = new("input2.txt", true);
        fileParser.ParseLinesToGrammarRules();
        fileParser.PrintGrammarRules();

        LLTableBuilder builder = new();
        Table table = builder.Build(fileParser.GrammarRules);

        LLTableCSVWriter.Write(table, "output.csv");
    }
}