namespace LLConverter_1
{
    public class LLTableBuilder()
    {
        private const string EMPTY_CHAR = "e";
        private const string END_CHAR = "@";

        public Table Build(List<GrammarRule> grammarRules)
        {
            // Разбор левой части правил
            var leftRows = ParseLeftPart(grammarRules);

            // Разбор правой части правил
            var rightRows = ParseRightPart(grammarRules);

            return new Table((leftRows.Concat(rightRows)).ToList());
        }

        private List<Row> ParseLeftPart(List<GrammarRule> grammarRules)
        {
            var result = new List<Row>();
            int ptr = grammarRules.Count;
            string nextToken = String.Empty;
            
            // Иду по каждому правилу

            // Нет ошибки, если есть следующее правило из такого же нетерминала
            for (int i = 0; i < grammarRules.Count; i++)
            {
                // Изначально ошибка есть
                bool error = true;
                

                // Если правило не последнее, то можно найти нетерминал (токен) следующего правила
                if ((i + 1) < grammarRules.Count)
                {
                    nextToken = grammarRules[i + 1].Token;
                }
                
                // Если следующий токен равен токену из разбираемого правила, то ошибка
                if (nextToken == grammarRules[i].Token)
                {
                    error = false;
                }
                // Иначе следующий тоекн это актуальный токен
                else
                {
                    nextToken = grammarRules[i].Token;
                }

                // Если нет ошибки и правило последнее, то ошибка есть
                if (i == grammarRules.Count - 1 && !error)
                {
                    error = true;
                }

                // Формируем строку: Токен, его направляющие символы, сдвиг (изначально -), ошибка, указатель (изначально = количеству правил), стэк (переходить ли на следующую строку? изначально -), конец (изначально -)
                var row = new Row(grammarRules[i].Token,
                    grammarRules[i].DirectionSymbols, false, error, ptr,
                    false, false);

                result.Add(row);
                // Добавляем к счётчику число символов у правила
                ptr += grammarRules[i].SymbolsChain.Count;
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
                        .Concat(rule.DirectionSymbols).Distinct()
                        .ToList();
                }
                else
                {
                    result.Add(rule.Token, rule.DirectionSymbols);
                }
            }
            return result;
        }

        private List<Row> ParseRightPart(List<GrammarRule> grammarRules)
        {
            // Терминал и все direction символы из него
            var dict = DoMapOfNonTerminal(grammarRules);

            var rows = new List<Row>();

            for (int i = 0; i < grammarRules.Count; i++)
            {
                for (int j = 0; j < grammarRules[i].SymbolsChain.Count; j++)
                {
                    // Беру символ из правила
                    var symbol = grammarRules[i].SymbolsChain[j];

                    // Если символ - терминал
                    if (dict.ContainsKey(symbol))
                    {
                        // Указатель - номер правила из левой части
                        int ptr = grammarRules.FindIndex(r => r.Token == symbol);

                        bool isLastSymbol = j == grammarRules[i].SymbolsChain.Count - 1;
                        bool isFirstGrammarRule = i == 0;

                        // Если символ последний и правило не первое, то сдвиг на другую строку
                        bool moveToNextLine = isLastSymbol && !isFirstGrammarRule;

                        // У него из direction все символы из мапы
                        // Сдвиг никогда
                        // Ошибка всегда
                        // Указатель: number
                        // Стэк = moveToNextLine
                        // И End = false
                        var row = new Row(symbol,
                            dict[symbol], false, true, ptr, moveToNextLine, false);
                        rows.Add(row);
                    }
                    // Если нетерминал
                    else
                    {
                        // Если эпсилон, то...
                        if (symbol == EMPTY_CHAR)
                        {
                            var row = new Row(symbol, grammarRules[i]
                                .DirectionSymbols,
                                false, true, null, false, false);
                            rows.Add(row);
                        }
                        // Если символ конца, то...
                        else if (symbol == END_CHAR)
                        {
                            // У него из direction только он же сам
                            // Сдвиг всегда
                            // Ошибка всегда
                            // Стэк никогда
                            // И End = true
                            var row = new Row(symbol, [symbol],
                                true, true, null, false, true);
                            rows.Add(row);
                        }
                        // Если обычный символ, то...
                        else
                        {
                            // Вычисление указателя.
                            // Если символ не последний в наборе, то указатель = количеству правил + количество строк по правой части. Если последний, то null
                            int? ptr = j != grammarRules[i].SymbolsChain.Count - 1
                                ? rows.Count + grammarRules.Count + 1 : null;

                            // Из direction symbol только он же сам
                            // Сдвиг всегда
                            // Ошибка всегда
                            // Указатель: number || null
                            // Стэк никогда
                            // Конец никогда
                            var row = new Row(symbol, [symbol], true, true, ptr,
                                false, false);
                            rows.Add(row);
                        }

                    }
                }
            }
            return rows;
        }
    }
}