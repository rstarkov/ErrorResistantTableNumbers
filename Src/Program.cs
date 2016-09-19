using System;
using System.Collections.Generic;
using System.Linq;

namespace TableNumbers
{
    class Program
    {
        static void Main(string[] args)
        {
            // PRNG constants
            _modulus = 1000;
            _rejectionLimit = (uint.MaxValue / _modulus) * _modulus;

            // Best result storage
            int bestLength = 0;
            var bestAssignment = new List<int>();

            while (true)
            {
                // Greedy random search for numbers that don't "interfere" with any of the numbers that have already been added to the set.
                var unavailable = new bool[1000];
                var unavailableCount = 0;
                var assignment = new List<int>();
                while (unavailableCount < unavailable.Length)
                {
                    // Generate random numbers until one is found which has not been previously marked as one which would be a single digit error away from a number that's already in the set
                    int num;
                    while (true)
                    {
                        if (_bufferPos >= _buffer.Length)
                        {
                            _bufferPos = 0;
                            FillBuffer();
                        }
                        uint result = _buffer[_bufferPos];
                        _bufferPos++;
                        if (result < _rejectionLimit) // rejection sampling to keep the random numbers uniformly distributed
                        {
                            num = (int) (result % _modulus);
                            if (!unavailable[num])
                                break;
                        }
                    }

                    // Mark every number that differs by a single digit from the chosen number as unavailable
                    for (int digit = 0, mask = 1; digit < 3; digit++, mask *= 10)
                    {
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

                    // Add this number to the set
                    assignment.Add(num);
                }

                // Store / print if this set is at least as large as the largest set found so far
                if (bestLength <= assignment.Count)
                {
                    bestLength = assignment.Count;
                    bestAssignment = assignment;
                    Console.WriteLine($"\r\n{bestLength}\r\n{string.Join(",", bestAssignment.OrderBy(a => a))}");
                }
            }
        }

        // Fast XorShift PRNG - see http://roman.st/Article/Faster-Marsaglia-Xorshift-pseudo-random-generator-in-unsafe-C

        private static uint _modulus;
        private static uint _rejectionLimit;
        private static uint[] _buffer = new uint[65536];
        private static int _bufferPos = int.MaxValue;

        private static uint _x = 123456789;
        private static uint _y = 362436069;
        private static uint _z = 521288629;
        private static uint _w = 88675123;

        private static unsafe void FillBuffer()
        {
            uint x = _x, y = _y, z = _z, w = _w;
            fixed (uint* pbytes = _buffer)
            {
                uint* pbuf = pbytes;
                uint* pend = pbytes + _buffer.Length;
                while (pbuf < pend)
                {
                    uint tx = x ^ (x << 11);
                    uint ty = y ^ (y << 11);
                    uint tz = z ^ (z << 11);
                    uint tw = w ^ (w << 11);
                    *(pbuf++) = x = w ^ (w >> 19) ^ (tx ^ (tx >> 8));
                    *(pbuf++) = y = x ^ (x >> 19) ^ (ty ^ (ty >> 8));
                    *(pbuf++) = z = y ^ (y >> 19) ^ (tz ^ (tz >> 8));
                    *(pbuf++) = w = z ^ (z >> 19) ^ (tw ^ (tw >> 8));
                }
            }
            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }
    }
}
