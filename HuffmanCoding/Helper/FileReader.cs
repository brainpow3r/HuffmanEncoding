using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HuffmanCoding.Helper
{
    public static class FileReader
    {
        
        public static byte[] GetCharactersFromFile(string filepath)
        {
            return File.ReadAllBytes(filepath);
            /*List<char> _buffer = new List<char>();
            
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

            }*/

            //return _buffer;
        }

        public static Dictionary<string, int> ConstructFrequencyTable(byte[] _buffer, int k)
        {
            BitArray fileContent = new BitArray(_buffer);
            Dictionary<string, int> _frequencies = new Dictionary<string, int>();

            int i = 0;
            string key = "";
            while(i < fileContent.Count)
            {
                BitArray word = new BitArray(k);
                for (int j = 0; j < k; j++)
                    word[j] = fileContent[i + j];

                for (int w = 0; w < k; w++)
                    key += (word[w] == true) ? "1" : "0";

                //Console.WriteLine("Key: {0}", key);

                if (_frequencies.ContainsKey(key))
                {
                    _frequencies[key]++;
                }
                else
                {
                    _frequencies[key] = 1;
                }
                
                key = "";
                i += k;
            }


            /*foreach(char c in _buffer)
            {
                if (_frequencies.ContainsKey(c))
                {
                    _frequencies[c]++;
                } else
                {
                    _frequencies[c] = 1;
                }
            }*/

            return _frequencies;
        }
    }
}
