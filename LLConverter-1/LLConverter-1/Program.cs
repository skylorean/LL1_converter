using LLConverter_1;
using System.Text;

class Program
{
    public static void Main(string[] args)
    {
        FileParser fileParser = new("input.txt");
        fileParser.ParseLinesToGrammarRules();
        fileParser.PrintGrammarRules();

        LLTableBuilder builder = new();
        Table table = builder.Build(fileParser.GrammarRules);

        LLTableCSVWriter.Write(table, "output.csv");
    }
}