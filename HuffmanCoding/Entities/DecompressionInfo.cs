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

        public DecompressionInfo(int size, string info, short orderLevel)
        {
            SizeInBytes = size;
            DecompressionInfoString = info;
            OrderLevel = orderLevel;
        }
        public int SizeInBytes { get; set; }
        public string DecompressionInfoString { get; set; } = "";
        public short OrderLevel { get; set; } = 0;
    }
}
