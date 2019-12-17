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
        }

        public static (Dictionary<string, int> freq, string leftovers) ConstructFrequencyTable(byte[] _buffer, int wordLength)
        {
            BitArray fileContent = new BitArray(_buffer);
            Dictionary<string, int> _frequencies = new Dictionary<string, int>();

            int i = 0;
            string key = "";
            string leftovers = "";
            if (fileContent.Count % wordLength != 0)
            {
                while(i < fileContent.Count-wordLength)
                {
                    BitArray word = new BitArray(wordLength);
                    for (int j = 0; j < wordLength; j++)
                        word[j] = fileContent[i + j];

                    for (int w = 0; w < wordLength; w++)
                        key += (word[w] == true) ? "1" : "0";

                    if (_frequencies.ContainsKey(key))
                    {
                        _frequencies[key]++;
                    }
                    else
                    {
                        _frequencies[key] = 1;
                    }
                
                    key = "";
                    i += wordLength;
                }

                // return leftover bits as string
                for (int q = i; q < fileContent.Count; q++)
                {
                    leftovers += (fileContent[q] == true) ? "1" : "0";
                }

                return (_frequencies, leftovers);

            } 
            else
            {
                while (i < fileContent.Count)
                {
                    BitArray word = new BitArray(wordLength);
                    for (int j = 0; j < wordLength; j++)
                        word[j] = fileContent[i + j];

                    for (int w = 0; w < wordLength; w++)
                        key += (word[w] == true) ? "1" : "0";

                    if (_frequencies.ContainsKey(key))
                    {
                        _frequencies[key]++;
                    }
                    else
                    {
                        _frequencies[key] = 1;
                    }

                    key = "";
                    i += wordLength;
                }

                return (_frequencies, null);
            }
        }
    }
}
