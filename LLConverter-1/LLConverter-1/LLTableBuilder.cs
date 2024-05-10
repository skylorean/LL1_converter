using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLConverter_1
{
    public class LLTableBuilder()
    {
        private const string EMPTY_CHAR = "e";
        private const string END_CHAR = "@";

        //public List<GrammarRule> GrammarRules { get; private set; } = grammarRules;

        public Table Build(List<GrammarRule> grammarRules)
        {
            //var ptrsLeftPart = new List<int>();
            var leftRows = ParseLeftPart(grammarRules);
            var rightRows = ParseRightPart(grammarRules);
            //for (int i = 0; i < leftRows.Count; i++)
            //{
            //    leftRows[i].Pointer = ptrsLeftPart[i];
            //}

            return new Table((leftRows.Concat(rightRows)).ToList());
        }

        private List<Row> ParseLeftPart(List<GrammarRule> grammarRules)
        {
            var result = new List<Row>();
            int ptr = grammarRules.Count;
            string nextToken = String.Empty;
            for (int i = 0; i < grammarRules.Count; i++)
            {
                bool error = true;
                if ((i + 1) < grammarRules.Count)
                {
                    nextToken = grammarRules[i + 1].Token;
                }
                if (nextToken == grammarRules[i].Token)
                {
                    error = false;
                }
                else
                {
                    nextToken = grammarRules[i].Token;
                }
                if (i == grammarRules.Count - 1 && !error)
                {
                    error = true;
                }
                var row = new Row(grammarRules[i].Token,
                    grammarRules[i].DirectionSymbols, false, error, ptr,
                    false, false);

                result.Add(row);
                ptr += grammarRules[i].SymbolsChain.Count;
                //if (i == 0)
                //{
                //    ptr++;
                //}
            }
            return result;
        }

        private Dictionary<string, List<string>> DoMapOfNonTerminal(
            List<GrammarRule> grammarRules)
        {
            var result = new Dictionary<string, List<string>>();
            foreach (GrammarRule rule in grammarRules)
            {
                if (result.ContainsKey(rule.Token))
                {
                    result[rule.Token] = result[rule.Token]
                        .Concat(rule.DirectionSymbols)
                        .ToList();
                }
                else
                {
                    result.Add(rule.Token, rule.DirectionSymbols);
                }
            }
            return result;
        }

        private List<int> ParseRulesForEndChars(List<GrammarRule> grammarRules)
        {
            var result = new List<int>();
            var firstChar = grammarRules[0].Token;                        
            for (int i = 0; i < grammarRules.Count; i++)
            {
                if (grammarRules[i].Token != firstChar) 
                {
                    break;
                }
                result.Add(i);
            }
            return result;
        }

        private List<Row> ParseRightPart(List<GrammarRule> grammarRules)
        {
            var dict = DoMapOfNonTerminal(grammarRules);
            //var endsIdx = ParseRulesForEndChars(grammarRules);
            var rows = new List<Row>();
            for (int i = 0; i < grammarRules.Count; i++)
            {
                for (int j = 0; j < grammarRules[i].SymbolsChain.Count; j++)
                {
                    var symbol = grammarRules[i].SymbolsChain[j];                    
                    if (dict.ContainsKey(symbol))
                    {
                        var ptr = grammarRules.FindIndex(r => r.Token == symbol);
                        bool moveToNextLine = true;
                        if (j == grammarRules[i].SymbolsChain.Count - 1 && i != 0)
                        {
                            moveToNextLine = false;
                        }
                        //var moveToNextLine = j == GrammarRules.
                        //var moveToNextLine = j == GrammarRules[i].SymbolsChain.Count - 1
                        //    ? false : true;
                        //var end = i == rules.Count - 1
                        var row = new Row(symbol,
                            dict[symbol], false, true, ptr, moveToNextLine, false);
                        rows.Add(row);
                    }
                    else
                    {
                        bool moveToNextLine = j == grammarRules[i]
                            .SymbolsChain.Count - 1
                                ? false : true;
                        if (symbol == EMPTY_CHAR)
                        {
                            var row = new Row(symbol, grammarRules[i]
                                .DirectionSymbols,
                                false, true, null, false, false);
                            rows.Add(row);
                        }
                        else if (symbol == END_CHAR)
                        {
                            var row = new Row(symbol, [symbol], 
                                true, true, null, false, true);
                            rows.Add(row);
                        }
                        else
                        {
                            var directions = new List<string>(1)
                            {
                            symbol
                            };
                            int? ptr = j != grammarRules[i].SymbolsChain.Count - 1
                                ? rows.Count + grammarRules.Count + 1 : null;
                            var row = new Row(symbol, directions, true, true, ptr,
                                false, false);
                            rows.Add(row);
                        }

                    }
                    //if (endsIdx.Contains(i) && j == GrammarRules[i].SymbolsChain.Count - 1)
                    //{
                    //    var row = new Row(END_CHAR,
                    //        [END_CHAR], true, true, null, false, true);
                    //    rows.Add(row);
                    //}
                }
                //int ptrLeftPart = rows.Count - GrammarRules[i]
                //    .SymbolsChain.Count + GrammarRules.Count;
                //if (endsIdx.Contains(i))
                //{
                //    ptrLeftPart--;
                //}    
                //leftPartsPtrs.Add(ptrLeftPart);
            }
            return rows;
        }
    }
}
