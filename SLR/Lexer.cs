using SLRConverter;

namespace SLRConverter
{
    public class Lexer
    {
        public string NUMBER_TOKEN = "number";
        public string ID_TOKEN = "id";
        public string STRING_TOKEN = "string";

        public List<string> ACCEPTABLE_SYMBOLS = [
            ",", ";", "(", ")", "-", "+", "*"];

        private List<string> _words = [];
        private int _currWordIndex = 0;

        public Lexer(string fileName)
        {
            // Парсинг входной строки
            string[] lines = FileParser.ReadFile(fileName);

            foreach (string line in lines)
            {
                foreach (string word in line.Split(' '))
                {
                    if (word.Length > 0 && word != " ")
                    {
                        _words.Add(word);
                    }
                }
            }

            // Добавление символа конца строки
            _words.Add("@");
        }

        // Получение следующего токена из входной цепочки
        public string GetNextToken()
        {
            // Если конец входной цепочки, то..
            if (IsEnd())
            {
                throw new Exception("File is ended");
            }

            // Текущий токен входной цепочки
            string currWord = _words[_currWordIndex++];

            if (currWord == "@")
            {
                return "@";
            }

            // Если number
            if (double.TryParse(currWord, out double _))
            {
                return NUMBER_TOKEN;
            }

            // Если string
            bool isString = currWord.StartsWith('"') && currWord.EndsWith('"') && currWord.Length > 1;
            if (isString) return STRING_TOKEN;

            // Если доп. символы
            if (ACCEPTABLE_SYMBOLS.Contains(currWord)) return currWord;

            // Дальше может быть только id
            // Если первый символ НЕ буква или НЕ _, то...
            if (!char.IsLetter(currWord[0]) && currWord[0] != '_'
                )
            {
                throw new Exception($"Token must start with a letter or _. Token index: {_currWordIndex}");
            }

            return ID_TOKEN;
        }

        // Конец ли входной цепочки
        public bool IsEnd()
        {
            return _currWordIndex == _words.Count;
        }
    }
}