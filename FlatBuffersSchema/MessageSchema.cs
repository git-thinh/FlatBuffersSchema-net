using System;
using System.Collections.Generic;

namespace FlatBuffers.Schema
{
    public sealed class MessageSchema
    {
        public delegate Table MessageCreator(ByteBuffer buffer);

        private Dictionary<int, MessageCreator> messages = new Dictionary<int, MessageCreator>();

        public void Register(int messageId, MessageCreator creator)
        {
            if (messages.ContainsKey(messageId))
                throw new InvalidOperationException(string.Format("Message #{0} is already defined", messageId));

            messages.Add(messageId, creator);
        }

        public Message Parse(int messageId, byte[] data)
        {
            MessageCreator creator;
            if (messages.TryGetValue(messageId, out creator))
            {
                var body = creator(new ByteBuffer(data));

                return new Message(messageId, body);
            }

            throw new InvalidOperationException(string.Format("Missing creator for message #{0}", messageId));
        }
    }
}