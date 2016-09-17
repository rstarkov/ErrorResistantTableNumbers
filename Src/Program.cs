﻿using System;
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
                if (trials > 500)
                {
                    Console.WriteLine((DateTime.UtcNow - start).TotalSeconds);
                    return;
                }
                var unavailable = new int[10000];
                var unavailableCount = 0;
                var assignment = new HashSet<int>();
                var table = 1;
                while (unavailableCount < unavailable.Length)
                {
                    var num = rnd.Next(0, unavailable.Length);
                    while (unavailable[num] > 0)
                        num = rnd.Next(0, unavailable.Length);
                    foreach (var variant in variants($"{num:0000}"))
                    {
                        int varianti = int.Parse(variant);
                        if (unavailable[varianti] == 0)
                            unavailableCount++;
                        unavailable[varianti] = table;
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

        static IEnumerable<string> variants(string num)
        {
            for (int i = 0; i <= 9; i++)
                yield return num.Remove(0, 1).Insert(0, i.ToString());
            for (int i = 0; i <= 9; i++)
                yield return num.Remove(1, 1).Insert(1, i.ToString());
            for (int i = 0; i <= 9; i++)
                yield return num.Remove(2, 1).Insert(2, i.ToString());
            for (int i = 0; i <= 9; i++)
                yield return num.Remove(3, 1).Insert(3, i.ToString());
        }
    }
}
