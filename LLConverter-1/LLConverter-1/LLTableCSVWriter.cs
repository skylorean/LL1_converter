using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLConverter_1
{
    public static class LLTableCSVWriter
    {
        private static string WriteBool(bool value)
        {
            return value ? "+" : "-";
        }
        private static List<string> GetHeadersOfTable()
        {
            return new List<string>
            {
                "N", "Symbol", "DirectionSymbols", "Shift", "Error",
                "Pointer", "Stack", "End"
            };
        }
        public static void Write(this Table table, string filePath)
        {
            if (Path.GetExtension(filePath) != ".csv")
            {
                throw new ArgumentException("File should be with extension .csv");
            }
            using (var writer = new StreamWriter(filePath, false,
                Encoding.Default))
            {
                writer.WriteLine(string.Join(";", GetHeadersOfTable()));
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string token = table.Rows[i].Token == ";" ? "semicolon" 
                        : table.Rows[i].Token;
                    List<string> dirChars = table.Rows[i].DirectionSymbols;
                    //dirChars.ForEach(x => { if (x == ";") x = "semicolon"; });
                    //string directCharStr = string
                    //for (int j = 0; j < dirChars.Count; j++)
                    //{
                    //    if (dirChars[j] == ";")
                    //    {
                    //        dirChars[j] = "semicolon";
                    //    }
                    //}
                    //  .Join(",", table.Rows[i].DirectionSymbols);
                    //directCharStr
                    string line = i.ToString() + ";";                    
                    line += token + ";" +
                        string.Join(",", dirChars);
                    line += ";" + WriteBool(table.Rows[i].Shift) + ";";
                    line += WriteBool(table.Rows[i].Error) + ";";
                    line += (table.Rows[i].Pointer == null ? "null" : 
                        table.Rows[i].Pointer
                        .ToString()) + ";";
                    line += WriteBool(table.Rows[i].MoveToNextLine) + ";";
                    line += WriteBool(table.Rows[i].End) + ";";
                    writer.WriteLine(line);
                }
            }
        }
    }
}
