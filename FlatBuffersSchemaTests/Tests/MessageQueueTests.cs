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
            var pingBuffer = CreatePingMessage(count, msg);
            queue.Enqueue(Message.Serialize(MessageIds.Ping, pingBuffer));

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
            var msgPos = fbb.CreateString(msg);

            PingMessage.StartPingMessage(fbb);
            PingMessage.AddCount(fbb, count);
            PingMessage.AddMsg(fbb, msgPos);
            var ping = PingMessage.EndPingMessage(fbb);
            PingMessage.FinishPingMessageBuffer(fbb, ping);

            return fbb;
        }
    }
}