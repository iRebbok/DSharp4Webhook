using System;

namespace DSharp4Webhook.Buffers
{
    public readonly struct LongArraySegment<T>
    {
        private readonly T[] _array;
        private readonly long _offset;
        private readonly long _count;

        /// <inheritdoc cref="ArraySegment{T}.Array" />
        public T[] Array
        {
            get
            {
                Validate();

                return _array!;
            }
        }

        /// <inheritdoc cref="ArraySegment{T}.Offset" />
        public long Offset
        {
            get
            {
                Validate();

                return _offset;
            }
        }

        /// <inheritdoc cref="ArraySegment{T}.Count" />
        public long Count
        {
            get
            {
                Validate();

                return _count;
            }
        }

        public LongArraySegment(T[] arr)
        {
            if (arr is null)
                throw new ArgumentNullException(nameof(arr));

            _array = arr;
            _offset = 0;
            _count = arr.LongLength;
        }

        public LongArraySegment(T[] arr, long offset, long count) : this(arr)
        {
            if (arr.LongLength - offset < count)
                throw new ArgumentException("Offset less than count");

            _offset = offset;
            _count = count;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(_array is null
                            ? 0
                            : _array.GetHashCode() ^ _offset ^ _count);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is LongArraySegment<T> value)
                return Equals(value);
            else
                return false;
        }

        public bool Equals(LongArraySegment<T> obj)
        {
            return obj._array == _array && obj._offset == _offset && obj._count == _count;
        }

        public static bool operator ==(LongArraySegment<T> a, LongArraySegment<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(LongArraySegment<T> a, LongArraySegment<T> b)
        {
            return !(a == b);
        }

        private bool ValidateSafe()
        {
            return (_array is null && 0 == _offset && 0 == _count)
                || (!(_array is null) && _offset >= 0 && _count >= 0 && _offset + _count <= _array.LongLength);
        }

        private void Validate()
        {
            if (!ValidateSafe())
                throw new InvalidOperationException("LongArraySegment invalid");
        }
    }
}
