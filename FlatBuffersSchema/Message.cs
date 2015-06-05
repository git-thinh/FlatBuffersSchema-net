using System;

namespace FlatBuffers.Schema
{
    public sealed class Message
    {
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
