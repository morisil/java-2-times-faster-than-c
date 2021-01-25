﻿using System;
using System.Linq;

namespace csharp
{
    static class Program
    {
        private const int MaxPayloadSize = 10000;
        private const int InitialNodeCount = 1000;
        private const long MutationCount = 10000000L;
        private const int MaxMutationSize = 10;

        class Node
        {
            public Node Previous { get; private set; }
            public Node Next { get; private set; }
            public long Id { get; }
            public byte[] Payload { get; }

            public Node(long id)
            {
                Id = id;
                Payload = Enumerable.Repeat((byte) id, (int) (AlmostPseudoRandom(id) * (double) MaxPayloadSize)).ToArray();
            }

            public void Join(Node node)
            {
                Previous = node;
                Next = node;
                node.Previous = this;
                node.Next = this;
            }

            public void Delete()
            {
                Next.Previous = Previous;
                Previous.Next = Next;
            }

            public void Insert(Node node)
            {
                node.Next = Next;
                node.Previous = this;
                Next.Previous = node;
                Next = node;
            }

            // this needs to be here because without it memory leak :(
            ~Node()
            {
            }
        }

        private static double AlmostPseudoRandom(long ordinal)
        {
            return (Math.Sin(((double) ordinal) * 100000.0) + 1.0) % 1.0;
        }

        static void Main(string[] args)
        {
            long nodeId = 0;
            long mutationSeq = 0;
            var head = new Node(nodeId++);
            head.Join(new Node(nodeId++));
            for (var i = 2; i < InitialNodeCount; i++)
            {
                head.Insert(new Node(nodeId++));
            }

            long nodeCount = InitialNodeCount;
            for (long i = 0; i < MutationCount; i++)
            {
                var deleteCount = (int) (AlmostPseudoRandom(mutationSeq++) * (double) MaxMutationSize);
                if (deleteCount > (nodeCount - 2))
                {
                    deleteCount = (int) nodeCount - 2;
                }

                for (int j = 0; j < deleteCount; j++)
                {
                    var toDelete = head;
                    head = head.Previous;
                    toDelete.Delete();
                }

                nodeCount -= deleteCount;
                var insertCount = (int) (AlmostPseudoRandom(mutationSeq++) * (double) MaxMutationSize);
                for (var j = 0; j < deleteCount; j++)
                {
                    head.Insert(new Node(nodeId++));
                    head = head.Next;
                }

                nodeCount += insertCount;
            }

            long checksum = 0;
            var traveler = head;
            do
            {
                checksum += traveler.Id + traveler.Payload.Length;
                if (traveler.Payload.Length > 0)
                {
                    checksum += (SByte) traveler.Payload[0];        // byte in c# is unsigned, need to use Signed byte
                }
            } while (
                (traveler = traveler.Next) != head
            );

            Console.WriteLine("node count: " + nodeCount);
            Console.WriteLine("checksum: " + checksum);
        }
    }
}