using System;
using System.Collections;
using System.Collections.Generic;

namespace FlatBuffers.Schema
{
    sealed class ByteQueue : IEnumerable<byte>
    {
        Queue<byte[]> queue = new Queue<byte[]>();
        int passedBytes;

        public void Enqueue(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            queue.Enqueue(data);
        }

        public int? Dequeue()
        {
            var bytes = Dequeue(sizeof(int));

            if (bytes != null)
                return BitConverter.ToInt32(bytes, 0);
            else
                return null;
        }

        public byte[] Dequeue(int bytes)
        {
            if (!HasBytes(bytes))
                return null;

            var dest = new byte[bytes];
            int destIndex = 0;

            while (destIndex < bytes)
            {
                var src = queue.Peek();
                var copyBytes = Math.Min(src.Length - passedBytes, bytes - destIndex);

                Array.Copy(src, passedBytes, dest, destIndex, copyBytes);

                destIndex += copyBytes;
                passedBytes += copyBytes;

                if (passedBytes == src.Length)
                {
                    queue.Dequeue();
                    passedBytes = 0;
                }
            }

            return dest;
        }

        public bool HasBytes(int bytes)
        {
            if (bytes == 0)
                throw new ArgumentException("bytes must > 0");

            if (queue.Count == 0)
                return false;

            bytes += passedBytes;

            foreach (var data in queue)
            {
                bytes -= data.Length;

                if (bytes <= 0)
                    return true;
            }

            return false;
        }

        #region IEnumerable<byte>

        public IEnumerator<byte> GetEnumerator()
        {
            int i = passedBytes;

            foreach (var data in queue)
            {
                for (; i < data.Length; ++i)
                    yield return data[i];

                i = 0;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}