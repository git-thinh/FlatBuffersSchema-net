using FlatBuffers.Schema.Tests;

namespace FlatBuffers.Schema.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            //var tests = new ByteQueueTests();
            //tests.TestInt();

            var tests = new MessageQueueTests();
            tests.TestPingMessage();
        }
    }
}