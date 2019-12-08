using System;
using System.Collections.Generic;
using System.IO;
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
            Dictionary<char, int> _frequencyTable = FileReader.ConstructFrequencyTable(_fileText);
            PQueue<HuffmanNode> q = new PQueue<HuffmanNode>(_frequencyTable.Count);
            Utils.FillPriorityQueue(ref q, _frequencyTable);

           

            Console.WriteLine("Ended reading");
        }
    }
}
