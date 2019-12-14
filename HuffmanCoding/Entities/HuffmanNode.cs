using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HuffmanCoding.Entities
{
    // Basic component of Huffman tree
    public class HuffmanNode
    {
        public int Data;
        public string Character;

        public HuffmanNode Left;
        public HuffmanNode Right;

        public int Compare([AllowNull] HuffmanNode x, [AllowNull] HuffmanNode y)
        {
            //if (x == null || y == null)
            //    throw ArgumentNullException;

            return x.Data - y.Data;
        }
    }
}
