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
            
            
            while (true)
            {
                string userInput = "";
                Console.WriteLine("Enter 1 for compression\nEnter 2 for Decompression\nEnter 'e' to exit");
                userInput = Console.ReadLine();

                if (!userInput.Equals("1") && !userInput.Equals("2") && !userInput.Equals("e"))
                    continue;

                if (userInput.Equals("e"))
                    break;

                switch(int.Parse(userInput))
                {
                    case 1:
                        {
                            string orderLevel = "";
                            string filename = "";
                            FileInfo fileToCompress = null;

                            orderLevel = PromptUser_Parameter();
                            filename = PromptUser_FileName();
                            filename = Path.Combine(_config.GetSection("DataFileDir").Value, filename);
                            
                            if (File.Exists(filename))
                                fileToCompress = new FileInfo(filename);

                            string _fileText = FileReader.GetCharactersFromFile(fileToCompress.FullName);
                            Dictionary<string, Dictionary<char, int>> contexts = new Dictionary<string, Dictionary<char, int>>();
                            
                            contexts = FileReader.ConstructFrequencyTable(_fileText, int.Parse(orderLevel));

                            Dictionary<string, PQueue<HuffmanNode>> q = new Dictionary<string, PQueue<HuffmanNode>>();
                            Utils.FillContextsPriorityQueues(ref q, contexts);
                            /*Utils.FillPriorityQueue(ref q, _frequencyTable);*/
                            HuffmanNode root = null;
                            Dictionary<string, Dictionary<char, string>> huffmanCodes = new Dictionary<string, Dictionary<char, string>>();

                            Dictionary<string, HuffmanNode> contextTrees = Utils.CreateHuffmanTreesForContexts(q);
                            //Utils.ConstructHuffmanTree(ref root, q);
                            Utils.GenerateHuffmanCodeTablesForContexts(ref huffmanCodes, contextTrees);

                            string resultsPath = Path.Combine(_config.GetSection("DataFileDir").Value, fileToCompress.FullName+".ggjs");

                            Utils.Compress(huffmanCodes, contexts, int.Parse(orderLevel), _fileText, resultsPath);
                        }
                        break;
                    case 2:
                        {
                            string filename = "";
                            FileInfo fileToDecompress = null;

                            filename = PromptUser_FileName();
                            filename = Path.Combine(_config.GetSection("DataFileDir").Value, filename);

                            if (File.Exists(filename))
                                fileToDecompress = new FileInfo(filename);

                            Utils.Decompress(fileToDecompress);

                            var files = Directory.GetFiles(_config.GetSection("OutputDirectory").Value);
                            var results = new FileInfo(files[0]);

                            Console.WriteLine("Reuslts File: {0}; Size: {1}", results.Name, results.Length);
                        }
                        break;
                    default:
                        {

                        }
                        break;
                }
            }
            

            

        }
        public static string PromptUser_Parameter()
        {
            Console.Write("Enter word length: ");
            return Console.ReadLine();
        }

        public static string PromptUser_FileName()
        {
            Console.Write("Enter filename to compress: ");
            return Console.ReadLine();
        }
    
    }

}
