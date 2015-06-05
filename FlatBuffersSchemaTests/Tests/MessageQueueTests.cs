using NUnit.Framework;
using System;

namespace FlatBuffers.Schema.Tests
{
    public class MessageQueueTests
    {
        private MessageSchema schema;
        private MessageQueue queue;

        public MessageQueueTests()
        {
            schema = new MessageSchema();
            queue = new MessageQueue(schema);
        }

        [Test]
        public void TestPingMessage()
        {
            // Register message creators
            schema.Register(MessageIds.Ping, PingMessage.GetRootAsPingMessage);
            schema.Register(MessageIds.Pong, PongMessage.GetRootAsPongMessage);

            var count = 10;
            var msg = "TestPing10";
            var ping = CreatePingMessage(count, msg);
            queue.Enqueue(ping.ToProtocolMessage(MessageIds.Ping));

            var message = queue.Dequeue();
            Assert.AreEqual((int)MessageIds.Ping, message.Id);

            var pingBody = message.Body as PingMessage;
            Assert.IsTrue(pingBody != null);
            Assert.AreEqual(count, pingBody.Count);
            Assert.AreEqual(msg, pingBody.Msg);
        }

        static FlatBufferBuilder CreatePingMessage(int count, string msg)
        {
            var fbb = new FlatBufferBuilder(1024);

            var ping = PingMessage.CreatePingMessage(fbb, count, fbb.CreateString(msg));
            PingMessage.FinishPingMessageBuffer(fbb, ping);

            return fbb;
        }
    }
}