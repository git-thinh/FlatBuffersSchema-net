/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 Wu Yuntao
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
*/

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
            private int reference;
            private Action<Message> processor;
            private bool once;

            public Processor(int reference, Action<Message> processor, bool once)
            {
                this.reference = reference;
                this.processor = processor;
                this.once = once;
            }

            public bool Invoke(Message message)
            {
                this.processor(message);

                return once;
            }

            public int Reference
            {
                get { return this.reference; }
            }
        }

        #endregion

        #region ProcessorSet

        class ProcessorSet
        {
            private List<Processor> processors = new List<Processor>();

            public void Process(Message message)
            {
                this.processors.RemoveAll(p => p.Invoke(message));
            }

            public void Attach(Processor processor)
            {
                this.processors.Add(processor);
            }

            public void Detach(int processor)
            {
                this.processors.RemoveAll(p => p.Reference == processor);
            }

            public void DetachAll()
            {
                this.processors.Clear();
            }
        }

        #endregion

        public MessageProcessor(MessageSchema schema)
        {
            this.messages = new MessageQueue(schema);
        }

        public void Enqueue(byte[] data)
        {
            this.messages.Enqueue(data);
        }

        public void Process()
        {
            foreach (var message in this.messages.DequeueAll())
            {
                var processors = GetProcessors(message.Id, false);
                if (processors != null)
                    processors.Process(message);
            }
        }

        public void Attach(int messageId, Action<Message> processor)
        {
            var processors = GetProcessors(messageId, true);

            processors.Attach(new Processor(processor.GetHashCode(), processor, false));
        }

        public void AttachOnce(int messageId, Action<Message> processor)
        {
            var processors = GetProcessors(messageId, true);

            processors.Attach(new Processor(processor.GetHashCode(), processor, true));
        }

        public void Detach(int messageId, Action<Message> processor)
        {
            var processors = GetProcessors(messageId, false);
            if (processors != null)
                processors.Detach(processor.GetHashCode());
        }

        public void Detach(int messageId)
        {
            var processors = GetProcessors(messageId, false);
            if (processors != null)
                processors.DetachAll();
        }

        ProcessorSet GetProcessors(int messageId, bool createWhenNotExist)
        {
            ProcessorSet processors;
            if (!this.processorSets.TryGetValue(messageId, out processors))
            {
                if (createWhenNotExist)
                {
                    processors = new ProcessorSet();
                    this.processorSets.Add(messageId, processors);
                }
            }

            return processors;
        }
    }
}