using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLConverter_1
{
    public class Lexer
    {
        private List<string> _words = [];
        private int _currWordIndex = 0;

        public Lexer(string fileName) 
        {
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
        }

        public string GetNextToken()
        {
            if (IsEnd())
            {
                throw new Exception("File is ended");
            }
            return _words[_currWordIndex++];
        }

        public bool IsEnd()
        {
            return _currWordIndex == _words.Count;
        }
    }
}