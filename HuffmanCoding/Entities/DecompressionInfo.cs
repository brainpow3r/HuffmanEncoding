using System;
using System.Collections.Generic;
using System.Text;

namespace HuffmanCoding.Entities
{
    public class DecompressionInfo
    {
        public DecompressionInfo()
        {
        }

        public DecompressionInfo(int size, string info, short wordLength, string leftoverBits, short lb_length)
        {
            SizeInBytes = size;
            DecompressionInfoString = info;
            WordLength = wordLength;
            LeftoverBits = leftoverBits;
            LeftoverBitsLength = lb_length;
        }
        public int SizeInBytes { get; set; }
        public string DecompressionInfoString { get; set; } = "";
        public short WordLength { get; set; } = 0;
        public string LeftoverBits { get; set; } = "";
        public short LeftoverBitsLength { get; set; } = 0;
    }
}
