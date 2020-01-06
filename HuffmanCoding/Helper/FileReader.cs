using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HuffmanCoding.Helper
{
    public static class FileReader
    {
        
        public static string GetCharactersFromFile(string filepath)
        {
            return File.ReadAllText(filepath);
        }

        public static Dictionary<string, Dictionary<char, int>> ConstructFrequencyTable(string text, int orderLevel)
        {
            
            Dictionary<string, Dictionary<char, int>> contextsTable = new Dictionary<string, Dictionary<char, int>>();

            string key = string.Empty;
            int substringStart = 0;
            
            for(int i = orderLevel;  i < text.Length; i++)
            {
                key = text.Substring(substringStart, orderLevel);

                if (contextsTable.ContainsKey(key))
                {
                    if (contextsTable[key].ContainsKey(text[i]))
                        contextsTable[key][text[i]]++;
                    else
                        contextsTable[key][text[i]] = 1;
                }
                else
                {
                    contextsTable[key] = new Dictionary<char, int>();
                    contextsTable[key][text[i]] = 1;
                }

                substringStart++;
                key = string.Empty;
            }

            return contextsTable;

        }
    }
}
