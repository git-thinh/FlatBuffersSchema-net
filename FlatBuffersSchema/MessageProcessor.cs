using System;
using System.Collections.Generic;

namespace FlatBuffers.Schema
{
    public sealed class MessageProcessor
    {
        private MessageQueue messages;
        private Dictionary<int, ProcessorSet> processorSets = new Dictionary<int, ProcessorSet>();

        #region Processor

        class Processor
        {
            private object reference;
            private Action<Message> processor;
            private bool once;

            public Processor(object reference, Action<Message> processor, bool once)
            {
                this.reference = reference;
                this.processor = processor;
                this.once = once;
            }

            public bool Invoke(Message message)
            {
                processor(message);

                return once;
            }

            public object Reference
            {
                get { return reference; }
            }
        }

        #endregion

        #region ProcessorSet

        class ProcessorSet
        {
            private List<Processor> processors = new List<Processor>();

            public void Process(Message message)
            {
                processors.RemoveAll(p => p.Invoke(message));
            }

            public void Attach(Processor processor)
            {
                processors.Add(processor);
            }

            public void Detach(object processor)
            {
                processors.RemoveAll(p => p.Reference == processor);
            }

            public void DetachAll()
            {
                processors.Clear();
            }
        }

        #endregion

        public MessageProcessor(MessageSchema schema)
        {
            messages = new MessageQueue(schema);
        }

        public void Enqueue(byte[] data)
        {
            messages.Enqueue(data);
        }

        public void Process()
        {
            foreach (var message in messages.DequeueAll())
            {
                var processors = GetProcessors(message.Id, false);
                if (processors != null)
                    processors.Process(message);
            }
        }

        public void Attach(int messageId, Action<Message> processor)
        {
            var processors = GetProcessors(messageId, true);

            processors.Attach(new Processor(processor, processor, false));
        }

        public void AttachOnce<TTable>(int messageId, Action<Message> processor)
        {
            var processors = GetProcessors(messageId, true);

            processors.Attach(new Processor(processor, processor, true));
        }

        public void Detach<TTable>(int messageId, Action<Message> processor)
        {
            var processors = GetProcessors(messageId, false);
            if (processors != null)
                processors.Detach(processor);
        }

        public void Detach<TTable>(int messageId)
        {
            var processors = GetProcessors(messageId, false);
            if (processors != null)
                processors.DetachAll();
        }

        ProcessorSet GetProcessors(int messageId, bool createWhenNotExist)
        {
            ProcessorSet processors;
            if (!processorSets.TryGetValue(messageId, out processors))
            {
                if (createWhenNotExist)
                {
                    processors = new ProcessorSet();
                    processorSets.Add(messageId, processors);
                }
            }

            return processors;
        }
    }
}