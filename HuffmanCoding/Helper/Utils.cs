using HuffmanCoding.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HuffmanCoding.Helper
{
    public static class Utils
    {
        public static void FillPriorityQueue(ref PQueue<HuffmanNode> q, Dictionary<char, int> fTable)
        {
            var enumerator = fTable.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                HuffmanNode hn = new HuffmanNode();

                hn.Character = enumerator.Current.Key;
                hn.Data = enumerator.Current.Value;

                hn.Right = null;
                hn.Left = null;

                q.Add(hn);

                //enumerator.MoveNext();

            }
        }

        public static void ConstructHuffmanTree(ref HuffmanNode root, PQueue<HuffmanNode> q)
        {
            while (q.Size() > 1)
            {
                HuffmanNode x = q.Peek();
                q.Pop();

                HuffmanNode y = q.Peek();
                q.Pop();

                HuffmanNode f = new HuffmanNode();

                f.Data = x.Data + y.Data;
                f.Character = '-';

                f.Left = x;
                f.Right = y;

                root = f;

                q.Add(f);
            }
        }

        public static void GenerateHuffmanCodeTable(ref Dictionary<char, string> huffmanCodes, ref HuffmanNode root, string s)
        {

            if (root.Left == null && root.Right == null)
            {
                huffmanCodes[root.Character] = s;

                return;
            }

            GenerateHuffmanCodeTable(ref huffmanCodes, ref root.Left, s + "0");
            GenerateHuffmanCodeTable(ref huffmanCodes, ref root.Right, s + "1");
            
        }

        public static void Compress(Dictionary<char, string> huffmanCodes, List<char> originalText, string outputFile)
        {
            if (File.Exists(outputFile))
                File.Delete(outputFile);

            using (FileStream f = new FileStream(outputFile, FileMode.OpenOrCreate))
            {
                f.Flush();
                byte[] textBytes = new byte[1000000];
                foreach (char letter in originalText)
                {
                    var codeInBytes = Encoding.UTF8.GetBytes(huffmanCodes[letter]);

                    //codeInBytes.CopyTo(textBytes);
                    
                    foreach(var bit in codeInBytes)
                    {
                        f.WriteByte(bit);
                    }
                }
            }
        }
    }
}
