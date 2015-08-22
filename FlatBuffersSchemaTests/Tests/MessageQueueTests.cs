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
            var lists = new int[][][]
            {
                new int[][]
                {
                    new int[] { 1, 2 },
                    new int[] { 2, 3 },
                },
                new int[][]
                {
                    new int[] { 3, 4 },
                    new int[] { 4, 5 },
                    new int[] { 5, 6 },
                },
                new int[][]
                {
                },

            };
            var ping = CreatePingMessage(count, msg, lists);
            queue.Enqueue(ping.ToProtocolMessage(MessageIds.Ping));

            var message = queue.Dequeue();
            Assert.AreEqual((int)MessageIds.Ping, message.Id);

            var pingBody = message.Body as PingMessage;
            Assert.IsTrue(pingBody != null);
            Assert.AreEqual(count, pingBody.Count);
            Assert.AreEqual(msg, pingBody.Msg);

            Assert.AreEqual(lists.Length, pingBody.ListsLength);

            Assert.AreEqual(lists[0].Length, pingBody.GetLists(0).ItemsLength);
            Assert.AreEqual(lists[0][0][0], pingBody.GetLists(0).GetItems(0).Key);
            Assert.AreEqual(lists[0][0][1], pingBody.GetLists(0).GetItems(0).Value);

            Assert.AreEqual(lists[1].Length, pingBody.GetLists(1).ItemsLength);
            Assert.AreEqual(lists[1][2][0], pingBody.GetLists(1).GetItems(2).Key);
            Assert.AreEqual(lists[1][2][1], pingBody.GetLists(1).GetItems(2).Value);

            Assert.AreEqual(lists[2].Length, pingBody.GetLists(2).ItemsLength);
        }

        static FlatBufferBuilder CreatePingMessage(int count, string msg, int[][][] lists)
        {
            var fbb = new FlatBufferBuilder(1024);

            var oLists = new Offset<PingList>[lists.Length];
            for (int i = 0; i < lists.Length; i++)
            {
                var list = lists[i];

                var oItems = new Offset<PingListItem>[list.Length];
                for (int j = 0; j < list.Length; j++)
                {
                    var item = list[j];

                    oItems[j] = PingListItem.CreatePingListItem(fbb, item[0], item[1]);
                }

                var voItems = PingList.CreateItemsVector(fbb, oItems);
                oLists[i] = PingList.CreatePingList(fbb, i, voItems);
            }

            var voLists = PingMessage.CreateListsVector(fbb, oLists);
            var oMsg = fbb.CreateString(msg);
            var oPing = PingMessage.CreatePingMessage(fbb, count, oMsg, voLists);
            PingMessage.FinishPingMessageBuffer(fbb, oPing);

            return fbb;
        }
    }
}