using System;
using System.Threading;

namespace FlatBuffers.Schema
{
    public sealed class Message
    {
        #region Serialization

        public const int InitialBufferSize = 1024;

        public static byte[] Serialize(int id, FlatBufferBuilder builder)
        {
            var buffer = builder.DataBuffer;
            var bufferSize = buffer.Length - buffer.Position;
            var bytes = new byte[bufferSize];
            Array.Copy(buffer.Data, buffer.Position, bytes, 0, bufferSize);

            return SerializeProtocolMessage(id, bytes);
        }

        public static byte[] Serialize<TEnum>(TEnum id, FlatBufferBuilder builder)
            where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("Type of id must be an enum");

            var intId = ((IConvertible)id).ToInt32(Thread.CurrentThread.CurrentCulture);

            return Serialize(intId, builder);
        }

        static byte[] SerializeProtocolMessage(int id, byte[] body)
        {
            var fbb = new FlatBufferBuilder(body.Length + InitialBufferSize);
            var bodyPos = ProtocolMessage.CreateBodyVector(fbb, body);

            ProtocolMessage.StartProtocolMessage(fbb);
            ProtocolMessage.AddId(fbb, id);
            ProtocolMessage.AddBody(fbb, bodyPos);
            var msg = ProtocolMessage.EndProtocolMessage(fbb);
            ProtocolMessage.FinishProtocolMessageBuffer(fbb, msg);

            var buffer = fbb.DataBuffer;
            var bufferSize = buffer.Length - buffer.Position;
            var head = BitConverter.GetBytes(bufferSize);

            var bytes = new byte[head.Length + bufferSize];
            Array.Copy(head, 0, bytes, 0, head.Length);
            Array.Copy(buffer.Data, buffer.Position, bytes, head.Length, bufferSize);

            return bytes;
        }

        #endregion

        private int id;
        private Table body;

        internal Message(int id, Table body)
        {
            this.id = id;
            this.body = body;
        }

        public int Id { get { return id; } }
        public Table Body { get { return body; } }
    }
}
