using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HuffmanCoding.Entities;
using HuffmanCoding.Helper;
using Lucene.Net.Util;
using Microsoft.Extensions.Configuration;

namespace HuffmanCoding
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json");
            var _config = builder.Build();
            string k = "";

            k = PromptUser_Parameter();
            Console.WriteLine("k is {0}", k);

            byte[] _fileText = FileReader.GetCharactersFromFile(_config.GetSection("DataFile").Value);
            Console.WriteLine("Total symbols: {0}", (_fileText.Length / 8));
            Console.WriteLine("sizeOf(char): {0}", sizeof(char));

            Console.WriteLine("Text byteCount: {0}", _fileText.Length);

            Dictionary<string, int> _frequencyTable = FileReader.ConstructFrequencyTable(_fileText, int.Parse(k));
            foreach (var entry in _frequencyTable)
            {
                Console.WriteLine("{0} : {1}", entry.Key, entry.Value);
            }
            PQueue<HuffmanNode> q = new PQueue<HuffmanNode>(_frequencyTable.Count);
            Utils.FillPriorityQueue(ref q, _frequencyTable);

            HuffmanNode root = null;
            Dictionary<string, BitArray> huffmanCodes = new Dictionary<string, BitArray>();

            Utils.ConstructHuffmanTree(ref root, q);
            Utils.GenerateHuffmanCodeTable(ref huffmanCodes, ref root, string.Empty);

            Console.WriteLine("Code lengths: ");
            foreach(var pair in huffmanCodes)
            {
                Console.WriteLine("Key: {0}; Code: {1}; Length: {2};", pair.Key, pair.Value, pair.Value.Count);
            }

            string outputDir = _config.GetSection("OutputDirectory").Value;
            string resultsPath = Path.Combine(outputDir, "results.txt");

            Utils.Compress(huffmanCodes, _frequencyTable, _fileText, resultsPath);
            Utils.Decompress(resultsPath, root, int.Parse(k));

            var files = Directory.GetFiles(_config.GetSection("OutputDirectory").Value);
            var results = new FileInfo(files[0]);

            Console.WriteLine("Reuslts File: {0}; Size: {1}", results.Name, results.Length);
        }
        public static string PromptUser_Parameter()
        {
            Console.Write("Enter desire value of k: ");
            return Console.ReadLine();
        }
    
    }

}
