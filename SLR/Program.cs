
using SLRConverter;

class Program
{
    public static void Main(string[] args)
    {
        FileParser fileParser = new("gr.txt", true);
        fileParser.ParseLinesToGrammarRules();
        fileParser.PrintGrammarRules();
        Console.WriteLine("Первая победа!");

        var table = SLRTableBuilder.Build(fileParser.GrammarRules);
        SLRTableCSVWriter.Write(table, "out.csv");
        Console.WriteLine("Вторая победа!");

        TableSlider tableSlider = new();
        tableSlider.RunSlider(table);
        Console.WriteLine("Третья Победа!");

        return;
    }
}