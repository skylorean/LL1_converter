using LLConverter_1;

class Program
{
    public static void Main(string[] args)
    {

        Console.WriteLine(args.Length);
        if (args.Length == 0)
        {
            throw new Exception("File path is not included");
        }

        string fileName = args[0];
        bool hasDirectionSymbols = args.Length == 1;
        Console.WriteLine(hasDirectionSymbols);

        FileParser fileParser = new(fileName, hasDirectionSymbols);
        fileParser.ParseLinesToGrammarRules();
        fileParser.PrintGrammarRules();

        LLTableBuilder builder = new();
        Table table = builder.Build(fileParser.GrammarRules);

        LLTableCSVWriter.Write(table, "output.csv");
    }
}