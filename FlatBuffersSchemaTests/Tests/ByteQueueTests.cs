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