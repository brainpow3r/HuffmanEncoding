using HuffmanCoding.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HuffmanCoding.Helper
{
    public class Huffman
    {
        public Huffman() { }

        public static void PrintCode(HuffmanNode root, string s)
        {
            if (root.Left == null && root.Right == null && !string.IsNullOrEmpty(root.Character))
            {
                Console.WriteLine(root.Character + " : " + s);
            }

            PrintCode(root.Left, s + "0");
            PrintCode(root.Right, s + "1");
        }
    }
}
