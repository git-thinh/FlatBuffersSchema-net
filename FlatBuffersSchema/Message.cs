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

namespace FlatBuffers.Schema
{
    public sealed class Message
    {
        public readonly string Type;
        public readonly object Object;

        internal Message(string type, object body)
        {
            Type = type;
            Object = body;
        }
    }

    public sealed class MessageSerializer : Serializer<Message, MessageEnvelope>
    {
        public static readonly MessageSerializer Instance = new MessageSerializer();

        public override Offset<MessageEnvelope> Serialize(FlatBufferBuilder fbb, Message msg)
        {
            var oType = fbb.CreateString(msg.Type);

            var bytes = SerializerSet.Instance.Serialize(msg.Type, msg.Object);
            var oBody = MessageEnvelope.CreateBodyVector(fbb, bytes);

            return MessageEnvelope.CreateMessageEnvelope(fbb, oType, oBody);
        }

        protected override MessageEnvelope GetRootAs(ByteBuffer buffer)
        {
            return MessageEnvelope.GetRootAsMessageEnvelope(buffer);
        }

        public override Message Deserialize(MessageEnvelope envelope)
        {
            var type = envelope.Type;
            var bytes = DeserializeScalar(envelope.BodyLength, envelope.Body);

            var obj = SerializerSet.Instance.Deserialize(type, bytes);

            return new Message(type, obj);
        }
    }
}
