using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

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
            if (bytes == 0)
                throw new ArgumentException("bytes must > 0");

            if (!HasBytes(bytes))
                return null;

            byte[] dest = new byte[bytes];
            int destIndex = 0;

            // Fetch first element
            {
                byte[] src = queue.Peek();

                destIndex = Math.Min(src.Length - passedBytes, bytes);
                Debug.Assert(destIndex > 0);

                Array.Copy(src, passedBytes, dest, 0, destIndex);

                if (destIndex == dest.Length)
                {
                    // Update src
                    passedBytes += destIndex;

                    if (passedBytes == src.Length)
                    {
                        queue.Dequeue();
                        passedBytes = 0;
                    }

                    return dest;
                }
            }

            // Fetch remaining
            int index = -1;
            foreach (var src in queue)
            {
                if (++index == 0)
                    continue;

                int copiedBytes = Math.Min(src.Length, dest.Length - destIndex);

                Array.Copy(src, 0, dest, destIndex, copiedBytes);

                destIndex += copiedBytes;

                if (destIndex == dest.Length)
                {
                    // Update src
                    if (copiedBytes == src.Length)
                    {
                        ++index;
                        passedBytes = 0;
                    }
                    else
                        passedBytes = copiedBytes;

                    // Remove elements before current
                    for (int i = 0; i < index; ++i)
                        queue.Dequeue();

                    return dest;
                }
            }

            return null;
        }

        public bool HasBytes(int bytes)
        {
            Debug.Assert(bytes > 0);

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