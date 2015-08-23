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

using NUnit.Framework;
using System;
using System.Text;

namespace FlatBuffers.Schema.Tests
{
    public class ByteQueueTests
    {
        [Test]
        public void TestInt()
        {
            var value = 21;
            var bytes = BitConverter.GetBytes(value);
            var queue = new ByteQueue();
            
            // Enqueue first two bytes of int
            var buffer = new byte[2];
            Array.Copy(bytes, 0, buffer, 0, 2);
            queue.Enqueue(buffer);

            Assert.IsTrue(queue.HasBytes(2));
            Assert.IsFalse(queue.HasBytes(3));

            Assert.IsNull(queue.Dequeue());

            // Enqueue last two bytes of int
            buffer = new byte[2];
            Array.Copy(bytes, 2, buffer, 0, 2);
            queue.Enqueue(buffer);

            Assert.IsTrue(queue.HasBytes(4));
            Assert.IsFalse(queue.HasBytes(5));

            var dequeuedValue = queue.Dequeue();
            Assert.AreEqual(value, dequeuedValue.Value);
            Assert.IsFalse(queue.HasBytes(1));
        }

        [Test]
        public void TestBytes()
        {
            var value = "TestStringBytes";
            var bytes = Encoding.UTF8.GetBytes(value);
            var queue = new ByteQueue();
            queue.Enqueue(bytes);

            Assert.IsTrue(queue.HasBytes(bytes.Length));
            Assert.IsFalse(queue.HasBytes(bytes.Length + 1));

            var dequeuedBytes = queue.Dequeue(bytes.Length);
            var dequeuedValue = Encoding.UTF8.GetString(dequeuedBytes);
            Assert.AreEqual(value, dequeuedValue);
        }
    }
}