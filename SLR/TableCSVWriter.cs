using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLRConverter
{
    //Класс для записи Таблицы в csv формате
    public static class SLRTableCSVWriter
    {
        //Поле для заголовков таблицы
        private static List<string> _tableHeaders = [];
        private static List<string> GetHeadersOfTable(Table table)
        {
            if (_tableHeaders.Count != 0)
            {
                return _tableHeaders;
            }
            List<string> result = ["N", "Name"];
            result.AddRange(table.ColumnNames);
            _tableHeaders = result;
            return result;
        }
        private static string GetRowString(int rowNumber, string rowName, Row row)
        {
            List<string> rowString = [];
            for (int i = 0; i < _tableHeaders.Count; i++)
            {
                rowString.Add(string.Empty);
            }
            if (rowNumber == 0)
            {
                int rootIdx = _tableHeaders.IndexOf(rowName);
                if (rootIdx != -1)
                {
                    rowString[rootIdx] = "Ok";
                }
            }
            rowString[0] = rowNumber.ToString();
            string name = rowName;
            if (name.Contains(","))
            {
                name = name.Replace(",", "comma");
            }
            else if (name.Contains(";"))
            {
                name = name.Replace(";", "semicolon");
            }
            rowString[1] = name;
            foreach (var key in row.Cells.Keys)
            {
                int idx = _tableHeaders.IndexOf(key);
                if (idx != -1)
                {
                    rowString[idx] = GetTableCellString(row.Cells[key]);
                }
            }
            return string.Join(";", rowString);
        }
        private static string GetTableCellString(TableCell cell)
        {
            return cell.Shift ? cell.Number.ToString() : $"R{cell.Number}";
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
                writer.WriteLine(string.Join(";", GetHeadersOfTable(table)));
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string rowName = string.Empty;
                    for (int j = 0; j < table.RowNames[i].Count; j++)
                    {
                        string separator = j == table.RowNames[i].Count - 1 ? "" : " ";
                        var rowKey = table.RowNames[i][j];
                        if (i == 0)
                        {
                            rowName += $"{rowKey.Token}";
                            break;
                        }
                        rowName += $"{rowKey.Token}{rowKey.Row}{rowKey.Column}{separator}";
                    }
                    writer.WriteLine(GetRowString(i, rowName, table.Rows[i]));
                }
            }
        }
    }
}