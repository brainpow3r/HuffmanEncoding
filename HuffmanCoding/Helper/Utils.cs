using HuffmanCoding.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HuffmanCoding.Helper
{
    public static class Utils
    {
        public static void FillPriorityQueue(ref PQueue<HuffmanNode> q, Dictionary<string, int> fTable)
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
                f.Character = "-";

                f.Left = x;
                f.Right = y;

                root = f;

                q.Add(f);
            }
        }

        public static void GenerateHuffmanCodeTable(ref Dictionary<string, BitArray> huffmanCodes, ref HuffmanNode root, string s)
        {

            if (root.Left == null && root.Right == null)
            {
                bool[] arr = new bool[s.Length];
                
                for(int i = 0; i < s.Length; i++)
                {
                    arr[i] = s[i].Equals('1') ? true : false; 
                }
                BitArray code = new BitArray(arr);
                huffmanCodes[root.Character] = code;
                return;
            }

            GenerateHuffmanCodeTable(ref huffmanCodes, ref root.Left, s + "0");
            GenerateHuffmanCodeTable(ref huffmanCodes, ref root.Right, s + "1");
            
        }

        public static void Compress(Dictionary<string, BitArray> huffmanCodes, Dictionary<string, int> _frequencyTable, byte[] originalText, string outputFile)
        {
            if (File.Exists(outputFile))
                File.Delete(outputFile);

            long codeLength = 0;
            // calculate length of code
            foreach (var pair in huffmanCodes)
            {
                codeLength += _frequencyTable[pair.Key] * pair.Value.Length;
            }

            //Console.WriteLine("Code length in bits: {0}", codeLength);

            bool[] textBytes = new bool[codeLength];
            var textBitsList = new List<bool>();

            BitArray compressingTextBits = new BitArray(originalText);

            int wordLength = huffmanCodes.Keys.First().Length;
            
            int i = 0;

            while (i < compressingTextBits.Count)
            {
                string wordToCompress = "";
                for (int o = 0; o < wordLength; o++)
                {
                    wordToCompress += (compressingTextBits[i] == true) ? "1" : "0";
                    i++;
                }

                var code = huffmanCodes[wordToCompress];
                for(int j = 0; j < code.Count; j++)
                {
                    textBitsList.Add(code[j]);
                }

                if (i % 100000 == 0) Console.WriteLine("Traversed {0} bits...", i);
                    
            }

            BitArray textBits = new BitArray(textBitsList.ToArray());
            byte[] bytesToWrite = new byte[textBits.Length / wordLength + (textBits.Length % wordLength == 0 ? 0 : 1)];
            textBits.CopyTo(bytesToWrite, 0);
            File.WriteAllBytes(outputFile, bytesToWrite);
        }

        public static void Decompress(string inputFile, HuffmanNode codeTreeRoot, int wordLength)
        {
            byte[] fileBytes = File.ReadAllBytes(inputFile);
            List<bool> decodedTextBits = new List<bool>();
            
            BitArray bitmap = new BitArray(fileBytes);
            HuffmanNode iteratedNode = codeTreeRoot;
            int i = 0;
            string decodedText = "";


            while(i < bitmap.Count)
            {
                if(iteratedNode.Left == null && iteratedNode.Right == null)
                {
                    decodedText += iteratedNode.Character;
                    foreach(var bit in iteratedNode.Character)
                    {
                        decodedTextBits.Add((bit.Equals('1') ? true : false));
                    }
                    //Console.WriteLine("decoded char: {0}", iteratedNode.Character);
                    iteratedNode = codeTreeRoot;
                    continue;
                } else
                {
                    if (bitmap[i] == true)
                        iteratedNode = iteratedNode.Right;
                    else
                        iteratedNode = iteratedNode.Left;
                }
                i++;

                if (i % 100000 == 0) Console.WriteLine("Traversed {0} bits...", i);
            }

            BitArray rrr = new BitArray(decodedTextBits.ToArray());
            byte[] bytesToWrite = new byte[rrr.Length / wordLength + (rrr.Length % wordLength == 0 ? 0 : 1)];
            rrr.CopyTo(bytesToWrite, 0);
            decodedText = Encoding.ASCII.GetString(bytesToWrite);

            Console.WriteLine("Decoded text: {0}", decodedText);
        }
        public static string KeyToString(BitArray arr)
        {
            string s = "";

            for (int i = 0; i < arr.Length; i++) { 
                s += (arr[i] == true) ? "1" : "0";
            }

            return s;
        }
    }

    
}
