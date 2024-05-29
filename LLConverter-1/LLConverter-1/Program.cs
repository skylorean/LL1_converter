using LLConverter_1;

class Program
{
    public static void Main(string[] args)
    {
        //if (args.Length == 0)
        //{
        //    throw new Exception("File path is not included");
        //}

        //string fileName = args[0];
        //bool hasDirectionSymbols = args.Length > 0 && bool.TryParse(args[1], out bool result) && result;

        FileParser fileParser = new("input2-wo.txt", false);
        //FileParser fileParser = new(fileName, hasDirectionSymbols);
        fileParser.ParseLinesToGrammarRules();
        fileParser.PrintGrammarRules();

        LLTableBuilder builder = new();
        Table table = builder.Build(fileParser.GrammarRules);

        LLTableCSVWriter.Write(table, "output.csv");

        try
        {
            TableSlider slider = new("lexer2.txt");
            slider.RunSlider(table);

            Console.WriteLine("Победа");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}