using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HuffmanCoding.Helper
{
    public static class FileReader
    {
        
        public static List<char> GetCharactersFromFile(string filepath)
        {
            List<char> _buffer = new List<char>();
            
            using (StreamReader reader = new StreamReader(filepath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    foreach(char c in line)
                    {
                        _buffer.Add(c);
                    }
                }

            }

            return _buffer;
        }

        public static Dictionary<char, int> ConstructFrequencyTable(List<char> _buffer)
        {
            Dictionary<char, int> _frequencies = new Dictionary<char, int>();

            foreach(char c in _buffer)
            {
                if (_frequencies.ContainsKey(c))
                {
                    _frequencies[c]++;
                } else
                {
                    _frequencies[c] = 1;
                }
            }

            return _frequencies;
        }
    }
}
