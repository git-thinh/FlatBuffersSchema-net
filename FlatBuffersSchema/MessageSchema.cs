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
using System.Threading;

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

        public void Register<TEnum>(TEnum messageId, MessageCreator creator)
            where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("Type of messageId must be an enum");

            var intMessageId = ((IConvertible)messageId).ToInt32(Thread.CurrentThread.CurrentCulture);

            Register(intMessageId, creator);
        }

        internal Message Parse(int messageId, byte[] data)
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