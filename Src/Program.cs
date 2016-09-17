using System;
using System.Collections.Generic;
using System.Linq;

namespace TableNumbers
{
    class Program
    {
        static void Main(string[] args)
        {
            var rnd = new Random(12345);

            int bestLength = 0;
            int bestDupes = 0;
            int bestHighest = int.MaxValue;
            var bestAssignment = new HashSet<int>();

            int trials = 0;
            var start = DateTime.UtcNow;

            while (true)
            {
                trials++;
                if (trials > 5000)
                {
                    Console.WriteLine((DateTime.UtcNow - start).TotalSeconds);
                    return;
                }
                var unavailable = new bool[10000];
                var unavailableCount = 0;
                var assignment = new HashSet<int>();
                var table = 1;
                while (unavailableCount < unavailable.Length)
                {
                    var num = rnd.Next(0, unavailable.Length);
                    while (unavailable[num])
                        num = rnd.Next(0, unavailable.Length);
                    for (int digit = 0; digit < 4; digit++)
                        foreach (var variant in variants(num, digit))
                            if (!unavailable[variant])
                            {
                                unavailableCount++;
                                unavailable[variant] = true;
                            }
                    assignment.Add(num);
                    table++;
                }

                var dupes = assignment.Select(a => $"{a:0000}").Sum(a => (a[0] == a[1] ? 1 : 0) + (a[1] == a[2] ? 1 : 0) + (a[2] == a[3] ? 1 : 0));
                if (bestLength < table)
                {
                    bestLength = table;
                    bestAssignment = assignment;
                    bestHighest = assignment.Max();
                    bestDupes = dupes;
                    Console.WriteLine($"\r\n{bestLength}, {bestDupes}; {string.Join(",", bestAssignment.OrderBy(a => a))}");
                }
                else if (bestLength == table)
                {
                    //if (string.Compare(bestHighest, assignment.Max()) > 0)
                    if (bestDupes < dupes)
                    {
                        bestAssignment = assignment;
                        bestHighest = assignment.Max();
                        bestDupes = dupes;
                        Console.WriteLine($"\r\n{bestLength}, {bestDupes}; {string.Join(",", bestAssignment.OrderBy(a => a))}");
                    }
                }
            }
        }

        static IEnumerable<int> variants(int num, int digit)
        {
            int mask = 1;
            for (int i = 0; i < digit; i++)
                mask *= 10;

            num = num - (num % (mask * 10) - num % mask);

            for (int i = 0; i <= 9; i++)
                yield return num + i * mask;
        }
    }
}
