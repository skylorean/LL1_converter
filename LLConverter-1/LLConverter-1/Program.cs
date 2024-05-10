using LLConverter_1;
using System.Text;

class Program
{
    public static void Main(string[] args)
    {
        FileParser fileParser = new("input.txt", false);
        fileParser.ParseLinesToGrammarRules();
        fileParser.PrintGrammarRules();

        LLTableBuilder builder = new();
        Table table = builder.Build(fileParser.GrammarRules);

        LLTableCSVWriter.Write(table, "output.csv");

        //table.Write("out.csv"); 
        TableSlider slider = new();
        try
        {
            //slider.RunSlider(table);
            Console.WriteLine("all good");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}