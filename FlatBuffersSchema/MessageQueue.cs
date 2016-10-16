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

using System.Collections.Generic;

namespace FlatBuffers.Schema
{
    public sealed class MessageQueue
    {
        private MessageSchema schema;
        private ByteQueue bytes = new ByteQueue();
        private int pendingMessageSize;

        public MessageQueue(MessageSchema schema)
        {
            this.schema = schema;
        }

        public void Enqueue(byte[] data)
        {
            this.bytes.Enqueue(data);
        }

        public void Enqueue(byte[] data, int offset, int count)
        {
            this.bytes.Enqueue(data, offset, count);
        }

        public Message Dequeue()
        {
            if (this.pendingMessageSize == 0)
            {
                var size = this.bytes.Dequeue();
                if (size == null)
                    return null; // There're not enough bytes which can express a complete varint

                this.pendingMessageSize = size.Value;
            }

            if (this.pendingMessageSize > 0)
            {
                var data = this.bytes.Dequeue(this.pendingMessageSize);
                if (data != null)
                {
                    this.pendingMessageSize = 0;

                    var message = ProtocolMessage.GetRootAsProtocolMessage(new ByteBuffer(data));

                    var body = new byte[message.BodyLength];
                    for (var i = 0; i < body.Length; i++)
                        body[i] = message.Body(i);

                    return this.schema.Parse(message.Id, body);
                }
            }

            return null;
        }
        public IEnumerable<Message> DequeueAll()
        {
            for (var message = Dequeue(); message != null; message = Dequeue())
                yield return message;
        }

    }
}