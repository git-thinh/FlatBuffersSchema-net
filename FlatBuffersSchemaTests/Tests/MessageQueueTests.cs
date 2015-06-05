using NUnit.Framework;

namespace FlatBuffers.Schema.Tests
{
    public class MessageQueueTests
    {
        private MessageSchema schema = new MessageSchema();

        [Test]
        public void TestSchemaRegistration()
        {
            // Register message creators
            schema.Register(MessageIds.Ping, PingMessage.GetRootAsPingMessage);
            schema.Register(MessageIds.Pong, PongMessage.GetRootAsPongMessage);
        }
    }
}