using System;
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
            

            List<char> _fileText = FileReader.GetCharactersFromFile(_config.GetSection("DataFile").Value);
            Console.WriteLine("Total symbols: {0}", _fileText.Count);
            Console.WriteLine("sizeOf(char): {0}", sizeof(char));

            Console.WriteLine("Text byteCount: {0}", Encoding.UTF8.GetByteCount(_fileText.ToArray()));

            Dictionary<char, int> _frequencyTable = FileReader.ConstructFrequencyTable(_fileText);
            foreach(var entry in _frequencyTable)
            {
                Console.WriteLine("{0} : {1}", entry.Key, entry.Value);
            }
            PQueue<HuffmanNode> q = new PQueue<HuffmanNode>(_frequencyTable.Count);
            Utils.FillPriorityQueue(ref q, _frequencyTable);

            HuffmanNode root = null;
            Dictionary<char, string> huffmanCodes = new Dictionary<char, string>();

            Utils.ConstructHuffmanTree(ref root, q);
            Utils.GenerateHuffmanCodeTable(ref huffmanCodes, ref root, string.Empty);

            string outputDir = _config.GetSection("OutputDirectory").Value;
            string resultsPath = Path.Combine(outputDir, "results.txt");

            Utils.Compress(huffmanCodes, _fileText, resultsPath);

            var files = Directory.GetFiles(_config.GetSection("OutputDirectory").Value);
            var results = new FileInfo(files[0]);
            var original = new FileInfo(files[1]);

            Console.WriteLine("Reuslts File: {0}; Size: {1}", results.Name, results.Length);
            //Console.WriteLine("File: {0}; Size: {1}", original.Name, originalfileSize);
        }
    }
}
