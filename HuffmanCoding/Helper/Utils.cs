using HuffmanCoding.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HuffmanCoding.Helper
{
    public static class Utils
    {
        public static void FillPriorityQueue(ref PQueue<HuffmanNode> q, Dictionary<char, int> fTable)
        {
            var enumerator = fTable.GetEnumerator();

            for (int i = 0; i < fTable.Count; i++)
            {
                HuffmanNode hn = new HuffmanNode();

                hn.Character = enumerator.Current.Key;
                hn.Data = enumerator.Current.Value;

                hn.Right = null;
                hn.Left = null;

                q.Add(hn);

                enumerator.MoveNext();

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
                f.Character = '-';

                f.Left = x;
                f.Right = y;

                root = f;

                q.Add(f);
            }
        }

        public static Dictionary<char, string> GenerateHuffmanCodeTable(Dictionary<char, int> fTable)
        {
            var result = new Dictionary<char, string>();


        }
    }
}
