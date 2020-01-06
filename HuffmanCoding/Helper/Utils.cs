using HuffmanCoding.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace HuffmanCoding.Helper
{
    public static class Utils
    {
        public static void FillContextsPriorityQueues(ref Dictionary<string, PQueue<HuffmanNode>> q, Dictionary<string, Dictionary<char, int>> contexts)
        {
            foreach(KeyValuePair<string, Dictionary<char, int>> kvp in contexts)
            {
                q[kvp.Key] = new PQueue<HuffmanNode>(kvp.Value.Count);
                var enumerator = kvp.Value.GetEnumerator();
            
                while (enumerator.MoveNext())
                {
                    HuffmanNode hn = new HuffmanNode();

                    hn.Character = enumerator.Current.Key;
                    hn.Data = enumerator.Current.Value;

                    hn.Right = null;
                    hn.Left = null;

                    q[kvp.Key].Add(hn);

                    //enumerator.MoveNext();

                }

            }
        }

        public static Dictionary<string, HuffmanNode> CreateHuffmanTreesForContexts(Dictionary<string, PQueue<HuffmanNode>> q)
        {
            var result = new Dictionary<string, HuffmanNode>();

            foreach(KeyValuePair<string, PQueue<HuffmanNode>> kvp in q)
            {
                HuffmanNode root = null;

                if (kvp.Value.Size() > 1)
                {
                    while (kvp.Value.Size() > 1)
                    {
                        HuffmanNode x = kvp.Value.Peek();
                        kvp.Value.Pop();

                        HuffmanNode y = kvp.Value.Peek();
                        kvp.Value.Pop();

                        HuffmanNode f = new HuffmanNode();

                        f.Data = x.Data + y.Data;
                        f.Character = '-';

                        f.Left = x;
                        f.Right = y;

                        root = f;

                        kvp.Value.Add(f);
                    }
                } else
                {
                    HuffmanNode x = kvp.Value.Peek();
                    kvp.Value.Pop();

                    HuffmanNode f = new HuffmanNode();
                    f.Data = x.Data;
                    f.Character = x.Character;
                    f.Left = null;
                    f.Right = null;
                    root = f;
                    kvp.Value.Add(f);


                }

                result[kvp.Key] = root;
            }

            return result;
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

        public static void GenerateHuffmanCodeTablesForContexts(ref Dictionary<string, Dictionary<char, string>> huffmanCodes, 
            Dictionary<string, HuffmanNode> contextTrees)
        {
            foreach(KeyValuePair<string, HuffmanNode> kvp in contextTrees)
            {
                string s = "";
                if (huffmanCodes[kvp.Key] == null)
                    huffmanCodes[kvp.Key] = new Dictionary<char, string>();

                GenerateHuffmanCodeTable(huffmanCodes[kvp.Key], kvp.Value, s);
            
            }
        }

        public static void GenerateHuffmanCodeTable(Dictionary<char, string> huffmanCodes, HuffmanNode root, string s)
        {

            if (root.Left == null && root.Right == null)
            {
                huffmanCodes[root.Character] = s;
                return;
            }

            GenerateHuffmanCodeTable(huffmanCodes, root.Left, s + "0");
            GenerateHuffmanCodeTable(huffmanCodes, root.Right, s + "1");

        }

        public static void Compress(Dictionary<string, Dictionary<char, string>> huffmanCodes, 
            Dictionary<string, Dictionary<char, int>> contextFrequencies, int orderLevel, string originalText, string outputFile)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("\nStarting compression...");
            
            DecompressionInfo decInfo = ConstructDecompressionInfoObject(contextFrequencies, (short)orderLevel);
            
            if (File.Exists(outputFile))
                File.Delete(outputFile);

            var encodedText = "";

            var firstSymbols = new BitArray(System.Text.ASCIIEncoding.ASCII.GetBytes(originalText.Substring(0, orderLevel)));

            foreach(bool bit in firstSymbols)
            {
                encodedText += (bit == true) ? "1" : "0";
            }

            int contextPosition = 0;
            for(int i = orderLevel; i < originalText.Length; i++)
            {
                var currentSymbol = originalText[i];
                var currentContext = originalText.Substring(contextPosition, orderLevel);

                encodedText += huffmanCodes[currentContext][currentSymbol];
                contextPosition++;
            }
            
            BitArray textBits = new BitArray(encodedText.Length);
            for(int i = 0; i < encodedText.Length; i++)
            {
                textBits[i] = (encodedText[i].Equals('1')) ? true : false;
            }

            byte[] bytesToWrite = new byte[textBits.Length / 8 + (textBits.Length % 8 == 0 ? 0 : 1)];

            textBits.CopyTo(bytesToWrite, 0);

            using (FileStream stream = new FileStream(outputFile, FileMode.OpenOrCreate))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(decInfo.SizeInBytes);
                    writer.Write(decInfo.DecompressionInfoString.ToCharArray(), 0, decInfo.DecompressionInfoString.Length);
                    writer.Write(decInfo.OrderLevel);
                    writer.Write(bytesToWrite);
                    writer.Close();
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Compression finished in: {0}", stopwatch.ElapsedMilliseconds);

            Console.WriteLine("Ending compression...");
        }

        public static void Decompress(FileInfo inputFile)
        {
            Stopwatch stopwacth = new Stopwatch();
            stopwacth.Start();
            Console.WriteLine("Starting decompression...");

            DecompressionInfo di = new DecompressionInfo();
            HuffmanNode root = null;
            HuffmanNode iteratedNode = null;
            byte[] fileBytes = null;
            int bytesOfDataToSkip = 0;

            using (FileStream stream = new FileStream(inputFile.FullName, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    di.SizeInBytes = reader.ReadInt32();
                    di.DecompressionInfoString = Encoding.UTF8.GetString(reader.ReadBytes(di.SizeInBytes));
                    di.OrderLevel = reader.ReadInt16();
                    fileBytes = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
                    reader.Close();
                }
            }

            List<bool> decodedTextBits = new List<bool>();
            
            BitArray bitmap = new BitArray(fileBytes);
            BitArray encodedTextBits = new BitArray(bitmap.Length - di.OrderLevel * 8);
            BitArray firstSymbols = new BitArray(di.OrderLevel * 8);

            // copy first symbols to know starting context and what's left to encoded text array
            
            int index = 0;
            for (int p = 0; p < di.OrderLevel * 8; p++)
            {
                firstSymbols[index] = bitmap[p];
                index++;
            }
            index = 0;
            byte[] firstSymbolBytes = new byte[firstSymbols.Length / 8];
            firstSymbols.CopyTo(firstSymbolBytes, 0);
            string firstSymbolsString = Encoding.ASCII.GetString(firstSymbolBytes);

            for (int h = di.OrderLevel * 8; h < bitmap.Length; h++)
            {
                encodedTextBits[index] = bitmap[h];
                index++;
            }

            Dictionary<string, HuffmanNode> decompressionTrees = ConstructDecompressionTree(di.DecompressionInfoString);

            int i = 0;
            string currentContext = firstSymbolsString;
            string decodedText = currentContext;
            
            HuffmanNode treeByContext = decompressionTrees[currentContext];
            iteratedNode = treeByContext;

            for(int l = 0; l < encodedTextBits.Length; l++)
            {
                if(iteratedNode.Left == null && iteratedNode.Right == null)
                {
                    decodedText += iteratedNode.Character;
                    currentContext = decodedText.Substring(decodedText.Length - di.OrderLevel, di.OrderLevel);
                    // decoded one symbol, return tree node to root
                    iteratedNode = decompressionTrees[currentContext];
                    continue;
                } else
                {
                    if (encodedTextBits[l] == true)
                        iteratedNode = iteratedNode.Right;
                    else
                        iteratedNode = iteratedNode.Left;
                }
                l++;

            }

            
            /*BitArray tempTextBitsArray = new BitArray(decodedTextBits.ToArray());
            byte[] bytesToWrite = new byte[tempTextBitsArray.Length / 8 + (tempTextBitsArray.Length % 8 == 0 ? 0 : 1)];
            tempTextBitsArray.CopyTo(bytesToWrite, 0);
            decodedText = Encoding.ASCII.GetString(bytesToWrite);*/
            string outputFile = $"dec_{inputFile.Name.Substring(0, inputFile.Name.Length - 5)}";
            outputFile = Path.Combine(inputFile.DirectoryName, outputFile);
            File.WriteAllText(outputFile, decodedText);

            stopwacth.Stop();
            Console.WriteLine($"Decompression finished in: {stopwacth.ElapsedMilliseconds}");
            Console.WriteLine("Ending decompression...\n");

        }

        public static string BitArrayToString(BitArray arr)
        {
            string s = "";

            for (int i = 0; i < arr.Length; i++) { 
                s += (arr[i] == true) ? "1" : "0";
            }

            return s;
        }

        public static bool[] StringToBitArray(string str)
        {
            bool[] arr = new bool[str.Length];

            for (int i = 0; i < str.Length; i++)
            {
                arr[i] = (str[i].Equals('1')) ? true : false;
            }

            return arr;
        }

        public static DecompressionInfo ConstructDecompressionInfoObject (Dictionary<string, Dictionary<char, int>> contextFrequencies, short orderLevel)
        {
            string info = "";

            foreach(KeyValuePair<string, Dictionary<char, int>> pair in contextFrequencies)
            {
                string segment = "";
                segment = $"{pair.Key}-";
                foreach(KeyValuePair<char, int> kvp in pair.Value)
                {
                    segment += $"{kvp.Key}:{kvp.Value.ToString()};";
                }
                info += $"{segment}|";
            }

            int size = info.Length;

            return new DecompressionInfo(size, info, orderLevel);
        }

        public static Dictionary<string, HuffmanNode> ConstructDecompressionTree(string decompressionInfoString)
        {

            Dictionary<string, Dictionary<char, int>> contextFrequencyTable = new Dictionary<string, Dictionary<char, int>>();

            // remove last ';' symbol from decompression info string so it doesn't fuck up Split()
            decompressionInfoString = decompressionInfoString.Trim().Substring(0, decompressionInfoString.Length - 1);
            var segments = decompressionInfoString.Split('|');
            foreach(var segment in segments)
            {
                var contextWithFrequencies = segment.Split('-');
                var context = contextWithFrequencies[0];
                
                contextFrequencyTable[context] = new Dictionary<char, int>();

                contextWithFrequencies[1] = contextWithFrequencies[1].Trim().Substring(0, contextWithFrequencies[1].Length - 1);
                var frequencies = contextWithFrequencies[1].Split(';');
                foreach(var pair in frequencies)
                {
                    if (!pair.Equals(string.Empty))
                    {
                        var frequencyInContext = pair.Split(':');
                    
                        if (!frequencyInContext[0].Equals(string.Empty))
                            contextFrequencyTable[context][frequencyInContext[0].ElementAt(0)] = int.Parse(frequencyInContext[1]);
                        else
                        {
                            frequencyInContext[0] = " ";
                            contextFrequencyTable[context][frequencyInContext[0].ElementAt(0)] = int.Parse(frequencyInContext[1]);
                        }

                    }

                }
            }

            Dictionary<string, PQueue<HuffmanNode>> q = new Dictionary<string, PQueue<HuffmanNode>>();

            FillContextsPriorityQueues(ref q, contextFrequencyTable);

            Dictionary<string, HuffmanNode> contextTrees = CreateHuffmanTreesForContexts(q);

            return contextTrees;

        }


    }

    
}
