/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 Wu Yuntao
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace FlatBuffers.Schema
{
    sealed class ByteQueue : IEnumerable<byte>
    {
        private Queue<byte[]> queue = new Queue<byte[]>();
        private int passedBytes;

        public void Enqueue(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            this.queue.Enqueue(data);
        }

        public void Enqueue(byte[] data, int offset, int count)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (offset < 0 || offset >= data.Length)
                throw new ArgumentOutOfRangeException("offset");

            if (count <= 0 || offset + count >= data.Length)
                throw new ArgumentOutOfRangeException("length");

            var bytes = new byte[count];
            Array.Copy(data, offset, bytes, 0, count);

            this.queue.Enqueue(data);
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
                var src = this.queue.Peek();
                var copyBytes = Math.Min(src.Length - this.passedBytes, bytes - destIndex);

                Array.Copy(src, this.passedBytes, dest, destIndex, copyBytes);

                destIndex += copyBytes;
                this.passedBytes += copyBytes;

                if (this.passedBytes == src.Length)
                {
                    this.queue.Dequeue();
                    this.passedBytes = 0;
                }
            }

            return dest;
        }

        public bool HasBytes(int bytes)
        {
            if (bytes == 0)
                throw new ArgumentException("bytes must > 0");

            if (this.queue.Count == 0)
                return false;

            bytes += this.passedBytes;

            foreach (var data in this.queue)
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
            int i = this.passedBytes;

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