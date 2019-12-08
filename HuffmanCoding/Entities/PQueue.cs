using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace HuffmanCoding.Entities
{
    public class PQueue<T> : PriorityQueue<T> where T : HuffmanNode
    {
        public PQueue(int n)
        {
            Initialize(n);
        } 
            
        public override bool LessThan(T a, T b)
        {
            if (a.Data - b.Data < 0)
                return true;
            else return false;
        }

        public T Peek()
        {
            return this.Top();
        }

        public void PrintQueue()
        {
            Console.WriteLine("QUEUE");
            foreach(var t in this.heap)
            {
                if (t != null)
                    Console.WriteLine("{0} : {1}", t.Character, t.Data);
            }
        }
    }
}
