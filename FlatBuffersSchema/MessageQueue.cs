using System.Collections.Generic;
using System.Diagnostics;

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
            Debug.Assert(data != null);

            bytes.Enqueue(data);
        }

        public IEnumerable<Message> DequeueAll()
        {
            for (var message = Dequeue(); message != null; message = Dequeue())
                yield return message;
        }

        public Message Dequeue()
        {
            if (pendingMessageSize == 0)
            {
                var size = bytes.Dequeue();
                if (size == null)
                    return null; // There're not enough bytes which can express a complete varint

                pendingMessageSize = size.Value;
            }

            if (pendingMessageSize > 0)
            {
                var data = bytes.Dequeue(pendingMessageSize);
                if (data != null)
                {
                    pendingMessageSize = 0;

                    var message = ProtocolMessage.GetRootAsProtocolMessage(new ByteBuffer(data));

                    var body = new byte[message.BodyLength];
                    for (var i = 0; i < body.Length; i++)
                        body[i] = message.GetBody(i);

                    return schema.Parse(message.Id, body);
                }
            }

            return null;
        }
    }
}