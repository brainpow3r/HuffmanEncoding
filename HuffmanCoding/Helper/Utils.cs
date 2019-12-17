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

        public static void Compress(Dictionary<string, BitArray> huffmanCodes, Dictionary<string, int> _frequencyTable, byte[] originalText, string outputFile, string leftoverBits)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Starting compression...");
            int wordLength = huffmanCodes.Keys.First().Length;
            int i = 0;
            DecompressionInfo decInfo = ConstructDecompressionInfoObject(_frequencyTable, (short)wordLength, leftoverBits);
            
            if (File.Exists(outputFile))
                File.Delete(outputFile);

            long codeLength = 0;
            // calculate length of code
            foreach (var pair in huffmanCodes)
            {
                codeLength += _frequencyTable[pair.Key] * pair.Value.Length;
            }

            var textBitsList = new List<bool>();
            BitArray compressingTextBits = new BitArray(originalText);

            
            if (compressingTextBits.Count % wordLength == 0)
            {

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
                    
                }

            } else
            {
                while (i < compressingTextBits.Count-wordLength)
                {
                    string wordToCompress = "";
                    for (int o = 0; o < wordLength; o++)
                    {
                        wordToCompress += (compressingTextBits[i] == true) ? "1" : "0";
                        i++;
                    }

                    var code = huffmanCodes[wordToCompress];
                    for (int j = 0; j < code.Count; j++)
                    {
                        textBitsList.Add(code[j]);
                    }

                }
            }

            BitArray textBits = new BitArray(textBitsList.ToArray());
            byte[] bytesToWrite = new byte[textBits.Length / 8 + (textBits.Length % 8 == 0 ? 0 : 1)];

            textBits.CopyTo(bytesToWrite, 0);

            using (FileStream stream = new FileStream(outputFile, FileMode.OpenOrCreate))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(decInfo.SizeInBytes);
                    writer.Write(decInfo.WordLength);
                    writer.Write(decInfo.LeftoverBitsLength);
                    if (decInfo.LeftoverBitsLength > 0)
                    {
                        writer.Write(decInfo.LeftoverBits.ToCharArray(), 0, decInfo.LeftoverBitsLength);
                    }
                    writer.Write(decInfo.DecompressionInfoString.ToCharArray(), 0, decInfo.DecompressionInfoString.Length);
                    writer.Write(bytesToWrite);
                    writer.Close();
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Compression finished in: {0}", stopwatch.ElapsedMilliseconds);

            Console.WriteLine("Ending compression...\n");
        }

        public static void Decompress(FileInfo inputFile)
        {
            Console.WriteLine("Starting decompression...\n");

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
                    di.WordLength = reader.ReadInt16();
                    di.LeftoverBitsLength = reader.ReadInt16();
                    if (di.LeftoverBitsLength > 0)
                    {
                        di.LeftoverBits = Encoding.UTF8.GetString(reader.ReadBytes(di.LeftoverBitsLength));
                    }
                    di.DecompressionInfoString = Encoding.UTF8.GetString(reader.ReadBytes(di.SizeInBytes));
                    bytesOfDataToSkip = (int)reader.BaseStream.Position;
                    fileBytes = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
                    reader.Close();
                }
            }

            List<bool> decodedTextBits = new List<bool>();
            
            BitArray bitmap = new BitArray(fileBytes);
            ConstructDecompressionTree(di.DecompressionInfoString, ref root);

            int i = 0;
            string decodedText = "";
            iteratedNode = root;

            while(i < bitmap.Count)
            {
                if(iteratedNode.Left == null && iteratedNode.Right == null)
                {
                    decodedTextBits.AddRange(StringToBitArray(iteratedNode.Character));
                    
                    // decoded one symbol, return tree node to root
                    iteratedNode = root;
                    continue;
                } else
                {
                    if (bitmap[i] == true)
                        iteratedNode = iteratedNode.Right;
                    else
                        iteratedNode = iteratedNode.Left;
                }
                i++;

            }

            if (di.LeftoverBitsLength > 0)
                decodedTextBits.AddRange(StringToBitArray(di.LeftoverBits));

            BitArray tempTextBitsArray = new BitArray(decodedTextBits.ToArray());
            byte[] bytesToWrite = new byte[tempTextBitsArray.Length / 8 + (tempTextBitsArray.Length % 8 == 0 ? 0 : 1)];
            tempTextBitsArray.CopyTo(bytesToWrite, 0);
            decodedText = Encoding.ASCII.GetString(bytesToWrite);
            string outputFile = $"dec_{inputFile.Name.Substring(0, inputFile.Name.Length - 5)}";
            outputFile = Path.Combine(inputFile.DirectoryName, outputFile);
            File.WriteAllBytes(outputFile, bytesToWrite);

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

        public static DecompressionInfo ConstructDecompressionInfoObject (Dictionary<string, int> _frequencyTable, short wl, string leftoverBits)
        {
            string info = "";

            foreach(var pair in _frequencyTable)
            {
                string segment = "";
                segment = pair.Key + ':' + pair.Value.ToString() + ';';
                info += segment;
            }
            int size = info.Length;
            short lb_length = 0;
            if (leftoverBits != null)
                lb_length = (short)leftoverBits.Length;
            

            return new DecompressionInfo(size, info, wl, leftoverBits, lb_length);
        }

        public static void ConstructDecompressionTree(string decompressionInfoString, ref HuffmanNode root)
        {

            Dictionary<string, int> frequencyTable = new Dictionary<string, int>();

            // remove last ';' symbol from decompression info string so it doesn't fuck up Split()
            decompressionInfoString = decompressionInfoString.Trim().Substring(0, decompressionInfoString.Length - 1);
            var segments = decompressionInfoString.Split(';');
            foreach(var segment in segments)
            {
                var split = segment.Split(':');
                frequencyTable[split[0]] = int.Parse(split[1]);
            }

            PQueue<HuffmanNode> q = new PQueue<HuffmanNode>(frequencyTable.Count);
            FillPriorityQueue(ref q, frequencyTable);

            ConstructHuffmanTree(ref root, q);
            
        }


    }

    
}
