using System;

namespace Shortening.Services
{
    public interface IHasher<T>
    {
        byte[] GetHash(T input);
    }

    public sealed class Hasher<T> : IHasher<T>
    {
        public byte[] GetHash(T input)
        {
            int code = input.GetHashCode();
            return BitConverter.GetBytes(code);
        }
    }
}
