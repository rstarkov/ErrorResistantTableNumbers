using System;
using System.Collections.Generic;
using System.Linq;

namespace TableNumbers
{
    class Program
    {
        static void Main(string[] args)
        {
            _modulus = 10000;
            _rejectionLimit = (uint.MaxValue / _modulus) * _modulus;

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
                        if (result < _rejectionLimit)
                        {
                            num = (int) (result % _modulus);
                            if (!unavailable[num])
                                break;
                        }
                    }

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
            fixed (uint* pbytes = _buffer)
            {
                uint* pbuf = pbytes;
                uint* pend = pbytes + _buffer.Length;
                while (pbuf < pend)
                {
                    uint tx = _x ^ (_x << 11);
                    uint ty = _y ^ (_y << 11);
                    uint tz = _z ^ (_z << 11);
                    uint tw = _w ^ (_w << 11);
                    *(pbuf++) = _x = _w ^ (_w >> 19) ^ (tx ^ (tx >> 8));
                    *(pbuf++) = _y = _x ^ (_x >> 19) ^ (ty ^ (ty >> 8));
                    *(pbuf++) = _z = _y ^ (_y >> 19) ^ (tz ^ (tz >> 8));
                    *(pbuf++) = _w = _z ^ (_z >> 19) ^ (tw ^ (tw >> 8));
                }
            }
        }
    }
}
