using System;
using System.Collections.Generic;
using System.Linq;
using Xorshift;

namespace TableNumbers
{
    class Program
    {
        static void Main(string[] args)
        {
            var rnd = new RndXorshift(10000);

            int bestLength = 0;
            var bestAssignment = new List<int>();

            int trials = 0;
            var start = DateTime.UtcNow;

            while (true)
            {
                trials++;
                if (trials > 10000)
                {
                    Console.WriteLine((DateTime.UtcNow - start).TotalSeconds);
                    return;
                }
                var unavailable = new bool[10000];
                var unavailableCount = 0;
                var assignment = new List<int>();
                var table = 1;
                while (unavailableCount < unavailable.Length)
                {
                    var num = (int) rnd.Next();
                    while (unavailable[num])
                        num = (int) rnd.Next();
                    for (int digit = 0; digit < 4; digit++)
                    {
                        int mask = 1;
                        for (int i = 0; i < digit; i++)
                            mask *= 10;

                        int maskednum = num - (num % (mask * 10) - num % mask);

                        for (int i = 0; i <= 9; i++)
                        {
                            var variant = maskednum + i * mask;
                            if (!unavailable[variant])
                            {
                                unavailableCount++;
                                unavailable[variant] = true;
                            }
                        }
                    }
                    assignment.Add(num);
                    table++;
                }

                if (bestLength < table)
                {
                    bestLength = table;
                    bestAssignment = assignment;
                    Console.WriteLine($"\r\n{bestLength}\r\n{string.Join(",", bestAssignment.OrderBy(a => a))}");
                }
            }
        }
    }
}
