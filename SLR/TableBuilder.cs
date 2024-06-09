using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLRConverter
{
    public class SLRTableBuilder
    {
        // Пустой символ
        private static readonly string EMPTY_CHAR = "e";

        // Символ конца разбора
        private static readonly string END_CHAR = "@";

        // Построение SLR таблицы по списку грамматических правил
        public static Table Build(List<GrammarRule> grammarRules)
        {
            //Словарь рядов будущей таблицы
            Dictionary<int, Row> rows = [];
            int nextStateIdx = 0;

            //Словарь названй рядов таблицы
            Dictionary<int, List<RowKey>> statesDict = [];

            Stack<int> stack = new();

            //Создаем первое состояние
            statesDict.Add(nextStateIdx, [new RowKey
                {
                    Token = grammarRules[0].Token,
                    Column = -1,
                    Row = -1
                }]);
            rows.Add(nextStateIdx, new Row());
            stack.Push(nextStateIdx);
            nextStateIdx++;

            while (stack.Count > 0)
            {
                var top = stack.Pop();

                //Обработка первого символа
                if (top == 0)
                {
                    // Объединяем те символы, что могут объединиться из направляющих символов
                    var symbols = MergeSymbols(grammarRules[0].DirectionSymbols);
                    foreach (var symbol in symbols)
                    {
                        // Если конец разбора, то делаем свертку
                        if (symbol[0].Token == END_CHAR)
                        {
                            rows[top].Cells.Add(END_CHAR, new TableCell
                            {
                                Shift = false,
                                Number = symbol[0].Row
                            });
                            continue;
                        }

                        //Добавляем новое состояние
                        statesDict.Add(nextStateIdx, symbol);
                        rows.Add(nextStateIdx, new Row());
                        var tableCell = new TableCell
                        {
                            Number = nextStateIdx,
                            Shift = true
                        };

                        //Добавляем ячейку таблицы со сдвигом в ряд бывшей вершины стека 
                        rows[top].Cells.Add(symbol[0].Token, tableCell);
                        stack.Push(nextStateIdx);
                        nextStateIdx++;
                    }
                    continue;
                }

                //Получаем состояние, которое может быть из нескольких грамматических вхождений
                var listRowKey = statesDict[top];
                for (int k = 0; k < listRowKey.Count; k++)
                {
                    var item = listRowKey[k];

                    //Если текущее грам. вхождение не самое правое
                    if (item.Column != grammarRules[item.Row].SymbolsChain.Count - 1)
                    {
                        var nextToken = grammarRules[item.Row].SymbolsChain[item.Column + 1];

                        //Если конец разбора, то делаем свертку
                        if (nextToken == END_CHAR) 
                        {
                            rows[top].Cells.Add(END_CHAR, new TableCell
                            {
                                Number = item.Row,
                                Shift = false
                            });
                            continue;
                        }

                        // Новое состояние
                        var nextState = new RowKey
                        {
                            Row = item.Row,
                            Column = item.Column + 1,
                            Token = nextToken
                        };

                        //Если следующий токен терминал
                        if (!IsNonTerminal(nextState.Token))
                        {
                            int newIdx = GetIdxRowKey(statesDict, [nextState]);

                            //Если ещё нет такого сотояния то создаём его
                            if (newIdx == -1)
                            {
                                newIdx = nextStateIdx;
                                statesDict.Add(newIdx, [nextState]);
                                stack.Push(newIdx);
                                rows.Add(newIdx, new Row());
                                nextStateIdx++;
                            }
                            //Дбавляем ячейку со сдвигом
                            rows[top].Cells.Add(nextState.Token, new TableCell
                            {
                                Number = newIdx,
                                Shift = true
                            });

                        }
                        //Если следующий токен нетерминал
                        else
                        {
                            var dirChars = GetDirectyonsSymbolsByToken(grammarRules, nextState.Token);
                            int newIdx = GetIdxRowKey(statesDict, [nextState]);
                            bool doCell = true; // флаг для создания ячейки таблицы

                            if (newIdx == -1)
                            {
                                //Если текущая вершина уже делает сдвиг или свёртку по следующему символу
                                if (rows[top].Cells.ContainsKey(nextState.Token))
                                {
                                    int numSt = rows[top].Cells[nextState.Token].Number;
                                    // Добавляем новое грамматические вхождение в имя ряда
                                    statesDict[numSt].Add(nextState);
                                    doCell = false;
                                }
                                else
                                {
                                    // Создаем новый ряд
                                    newIdx = nextStateIdx;
                                    var mergeExistNonTerm =
                                    MergeNonTerminalsWithExistNonTerm(dirChars, nextState);
                                    statesDict.Add(newIdx, mergeExistNonTerm);
                                    stack.Push(newIdx);
                                    rows.Add(newIdx, new Row());
                                    nextStateIdx++;
                                }

                            }
                            if (doCell)
                            {
                                rows[top].Cells.Add(nextState.Token, new TableCell
                                {
                                    Number = newIdx,
                                    Shift = true
                                });
                            }
                            var mergedChars = MergeSymbols(dirChars);
                            foreach (var merged in mergedChars)
                            {
                                if (merged[0].Token == nextState.Token)
                                    continue;
                                int mergedIdx = GetIdxRowKey(statesDict, merged);

                                // Если такого ряда нет то создаём его
                                if (mergedIdx == -1)
                                {
                                    mergedIdx = nextStateIdx;
                                    statesDict.Add(mergedIdx, merged);
                                    stack.Push(mergedIdx);
                                    rows.Add(mergedIdx, new Row());
                                    nextStateIdx++;
                                }

                                // Если текущий ряд не сворачивается по этому символу добавляем ячейку в ряд
                                if (!rows[top].Cells.ContainsKey(merged[0].Token))
                                    rows[top].Cells.Add(merged[0].Token, new TableCell
                                    {
                                        Number = mergedIdx,
                                        Shift = true
                                    });
                            }
                        }
                    }
                    else
                    {
                        ConvolutionProcessing(grammarRules, rows, item, top);
                    }
                }

            }
            var listRows = ConvertDictRowToList(rows);
            var table = new Table(listRows, grammarRules)
            {
                RootName = grammarRules[0].Token,
                RowNames = ConvertDictRowKeyToList(statesDict),
                ColumnNames = GetAllGrammaticSymbol(grammarRules)
            };
            return table;
        }

        // Метод обработки свертки по item
        // currIdx - индекс текущего ряда 
        private static void ConvolutionProcessing(List<GrammarRule> grammarRules, Dictionary<int, Row> rows, RowKey item, int currIdx)
        {
            string token = grammarRules[item.Row].Token;
            List<string> wasInStack = [];
            //Стек для заворачивания в случае если токен самы правый в правиле
            Stack<string> rStack = new();
            rStack.Push(token);
            while (rStack.Count > 0)
            {
                var tmpTop = rStack.Pop();
                if (wasInStack.Contains(tmpTop))
                    continue;
                wasInStack.Add(tmpTop);
                foreach (var grammarRule in grammarRules)
                {
                    var tokenIdx = grammarRule.SymbolsChain.IndexOf(tmpTop);
                    if (tokenIdx == -1)
                        continue;
                    else if (tokenIdx == grammarRule.SymbolsChain.Count - 1)
                    {
                        rStack.Push(grammarRule.Token);
                    }
                    else
                    {
                        var ch = grammarRule.SymbolsChain[tokenIdx + 1];
                        // Если терминал делаем свертку по нему
                        if (!IsNonTerminal(ch))
                        {
                            rows[currIdx].Cells.Add(ch, new TableCell
                            {
                                Number = item.Row,
                                Shift = false
                            });
                        }
                        // Если нетерминал, то делаем свертку по нему и по его направляющим символам
                        else
                        {
                            List<string> listNonTerm = GetDirectyonsSymbolsByToken(grammarRules,
                                ch).ConvertAll(x => x.Token).Distinct().ToList();
                            var tableCell = new TableCell
                            {
                                Number = item.Row,
                                Shift = false
                            };
                            listNonTerm.ForEach(x =>
                            {
                                if (!rows[currIdx].Cells.ContainsKey(x))
                                {
                                    rows[currIdx].Cells.Add(x, tableCell);
                                }
                            });
                        }
                    }
                }
            }
        }

        // Преобразование словаря рядов в список
        private static List<Row> ConvertDictRowToList(Dictionary<int, Row> rows)
        {
            List<int> listIdx = rows.Keys.ToList().OrderBy(x => x).ToList();
            List<Row> result = [];
            listIdx.ForEach(x =>
            {
                result.Add(rows[x]);
            });
            return result;
        }

        // Преобразование имен рядов в список
        private static List<List<RowKey>> ConvertDictRowKeyToList(Dictionary<int, List<RowKey>> rowKeys)
        {
            List<int> listIdx = rowKeys.Keys.ToList().OrderBy(x => x).ToList();
            List<List<RowKey>> result = [];
            listIdx.ForEach(x =>
            {
                result.Add(rowKeys[x]);
            });
            return result;
        }

        // Получение всех символов грамматики
        private static List<string> GetAllGrammaticSymbol(List<GrammarRule> grammarRules)
        {
            List<string> result = [];
            grammarRules.ForEach(rule =>
            {
                result.Add(rule.Token);
                result.AddRange(rule.SymbolsChain);
            });
            result = result.Distinct().ToList();
            result.Remove(END_CHAR);
            result.Add(END_CHAR);
            result.Remove(EMPTY_CHAR);
            return result;
        }

        // Получение всех направляющих символо для нетерминала
        private static List<RowKey> GetDirectyonsSymbolsByToken(List<GrammarRule> grammarRules, string token)
        {
            List<RowKey> result = [];
            foreach (var rule in grammarRules)
            {
                if (rule.Token == token)
                {
                    result.AddRange(rule.DirectionSymbols);
                }
            }
            return result.Distinct().ToList();
        }

        // Объединие нетерминала с его направляющими символами у которых такой же токен
        private static List<RowKey> MergeNonTerminalsWithExistNonTerm(List<RowKey> nonTermianls, RowKey rowKey)
        {
            List<RowKey> result = [rowKey];
            foreach (var nonTermianl in nonTermianls)
            {
                if (nonTermianl.Token == rowKey.Token)
                {
                    if (nonTermianl.Row == rowKey.Row && nonTermianl.Column == rowKey.Column)
                    {
                        continue;
                    }
                    result.Add(nonTermianl);
                }
            }
            return result;
        }

        // Получения индекса имени ряда в словаре
        // Если такого имени не существует то возвращается -1
        private static int GetIdxRowKey(Dictionary<int, List<RowKey>> statesDict, List<RowKey> state)
        {

            foreach (var key in statesDict.Keys)
            {
                statesDict[key].Sort((a, b) => a.Token.CompareTo(b.Token));
                state.Sort((a, b) => a.Token.CompareTo(b.Token));
                for (int i = 0; i < statesDict[key].Count && i < state.Count; i++)
                {
                    if (statesDict[key][i].Row == state[i].Row && statesDict[key][i].Column == state[i].Column
                        && statesDict[key][i].Token == state[i].Token)
                    {
                        return key;
                    }
                }
            }
            return -1;
        }

        // Объединение тех символов, что могут быть объединены
        // Возвращаемый результат список имён, те что не были объединены, были обернуты в список
        private static List<List<RowKey>> MergeSymbols
            (List<RowKey> symbols)
        {
            List<string> alreadyMerged = [];
            List<List<RowKey>> result = [];
            int idxResult = 0;
            for (int i = 0; i < symbols.Count; i++)
            {
                if (alreadyMerged.Contains(symbols[i].Token))
                {
                    continue;
                }
                result.Add([symbols[i]]);
                idxResult++;
                foreach (RowKey tmp in symbols)
                {
                    if (tmp.Token == symbols[i].Token)
                    {
                        if (tmp.Row == symbols[i].Row && tmp.Column == symbols[i].Column)
                        {
                            continue;
                        }
                        result[idxResult >= result.Count ? result.Count - 1 : idxResult].Add(tmp);
                        if (!alreadyMerged.Contains(tmp.Token))
                            alreadyMerged.Add(tmp.Token);
                    }
                }
            }
            return result;
        }

        // Является ли символ нетерминалом
        private static bool IsNonTerminal(string token)
        {
            return token.Contains('<') && token.Contains('>');
        }

    }
}