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
            
            /*TEST*/
            for (int wl = 2; wl <= 16; wl++)
            {
                string testFileName = "test2.txt";
                testFileName = Path.Combine(_config.GetSection("DataFileDir").Value, testFileName);
                FileInfo fileToCompress = new FileInfo(testFileName);

                byte[] _fileText = FileReader.GetCharactersFromFile(fileToCompress.FullName);
                Dictionary<string, int> _frequencyTable = new Dictionary<string, int>();
                string leftoverBits = null;
                (_frequencyTable, leftoverBits) = FileReader.ConstructFrequencyTable(_fileText, wl);

                PQueue<HuffmanNode> q = new PQueue<HuffmanNode>(_frequencyTable.Count);
                Utils.FillPriorityQueue(ref q, _frequencyTable);

                HuffmanNode root = null;
                Dictionary<string, BitArray> huffmanCodes = new Dictionary<string, BitArray>();

                Utils.ConstructHuffmanTree(ref root, q);
                Utils.GenerateHuffmanCodeTable(ref huffmanCodes, ref root, string.Empty);

                string resultsPath = Path.Combine(_config.GetSection("CompressedFilesDirectory").Value, $"{wl.ToString()}_"+fileToCompress.Name + ".ggjs");
                FileInfo compressedFileInfo = new FileInfo(resultsPath);

                Utils.Compress(huffmanCodes, _frequencyTable, _fileText, resultsPath, leftoverBits);

                Console.WriteLine("Original file size and compressed file size: \t{0} -- {1}", fileToCompress.Length, compressedFileInfo.Length);
                Console.WriteLine("Word length: {0}", wl);
                double compressionRate = (double)compressedFileInfo.Length / fileToCompress.Length;
                Console.WriteLine("Compression rate: {0}", compressionRate);

            }
            
            var filesToDecompress = Directory.GetFiles(_config.GetSection("CompressedFilesDirectory").Value);
            foreach(string file in filesToDecompress)
            {
                FileInfo fileToDecompress = new FileInfo(file);
                Utils.Decompress(fileToDecompress);
            }


            /*TEST*/
            while (false)
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
                            string wordLength = "";
                            string filename = "";
                            FileInfo fileToCompress = null;

                            wordLength = PromptUser_Parameter();
                            filename = PromptUser_FileName();
                            filename = Path.Combine(_config.GetSection("DataFileDir").Value, filename);
                            
                            if (File.Exists(filename))
                                fileToCompress = new FileInfo(filename);

                            byte[] _fileText = FileReader.GetCharactersFromFile(fileToCompress.FullName);
                            Dictionary<string, int> _frequencyTable = new Dictionary<string, int>();
                            string leftoverBits = null;
                            (_frequencyTable, leftoverBits) = FileReader.ConstructFrequencyTable(_fileText, int.Parse(wordLength));

                            PQueue<HuffmanNode> q = new PQueue<HuffmanNode>(_frequencyTable.Count);
                            Utils.FillPriorityQueue(ref q, _frequencyTable);

                            HuffmanNode root = null;
                            Dictionary<string, BitArray> huffmanCodes = new Dictionary<string, BitArray>();

                            Utils.ConstructHuffmanTree(ref root, q);
                            Utils.GenerateHuffmanCodeTable(ref huffmanCodes, ref root, string.Empty);

                            string resultsPath = Path.Combine(_config.GetSection("DataFileDir").Value, fileToCompress.FullName+".ggjs");

                            Utils.Compress(huffmanCodes, _frequencyTable, _fileText, resultsPath, leftoverBits);
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
