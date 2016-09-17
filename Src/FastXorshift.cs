namespace Xorshift
{
    sealed class RndXorshift
    {
        public RndXorshift(uint maxExclusive)
        {
            _modulus = maxExclusive;
            _rejectionLimit = (uint.MaxValue / _modulus) * _modulus;
        }

        public uint Next()
        {
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
                    return result % _modulus;
            }
        }

        private uint _modulus;
        private uint _rejectionLimit;
        private uint[] _buffer = new uint[65536];
        private int _bufferPos = int.MaxValue;

        private uint _x = 123456789;
        private uint _y = 362436069;
        private uint _z = 521288629;
        private uint _w = 88675123;

        private unsafe void FillBuffer()
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
