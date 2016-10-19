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
    public sealed class SerializerSet
    {
        public static readonly SerializerSet Instance = new SerializerSet();

        private readonly Dictionary<string, ISerializer> serializers = new Dictionary<string, ISerializer>();

        internal SerializerSet()
        { }

        public TSerializer CreateSerializer<TSerializer, TObject, TFlatBufferObject>()
            where TSerializer : ISerializer<TObject, TFlatBufferObject>, ISerializer, new()
            where TFlatBufferObject : struct, IFlatbufferObject
        {
            var serializer = new TSerializer();
            AddSerializer(typeof(TObject), serializer);
            return serializer;
        }

        public void AddSerializer(Type type, ISerializer serializer)
        {
            serializers.Add(type.FullName, serializer);
        }

        public ISerializer GetSerializer(Type type)
        {
            return GetSerializer(type.FullName);
        }

        private ISerializer GetSerializer(string typeName)
        {
            ISerializer serializer;
            serializers.TryGetValue(typeName, out serializer);
            return serializer;
        }

        public byte[] Serialize(Type type, object obj)
        {
            return Serialize(type.FullName, obj);
        }

        public byte[] Serialize(string typeName, object obj)
        {
            var serializer = GetSerializer(typeName);
            if (serializer == null)
                throw new InvalidOperationException(string.Format("Serializer of '{0}' does not exist", typeName));

            return serializer.Serialize(obj);
        }

        public object Deserialize(Type type, byte[] data)
        {
            return Deserialize(type.FullName, data);
        }

        public object Deserialize(string typeName, byte[] data)
        {
            var serializer = GetSerializer(typeName);
            if (serializer == null)
                throw new InvalidOperationException(string.Format("Serializer of '{0}' does not exist", typeName));

            return serializer.Deserialize(data);
        }
    }
}
