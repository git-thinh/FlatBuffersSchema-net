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
using System.Reflection;
using System.Threading;

namespace FlatBuffers.Schema
{
    public sealed class SerializerSet
    {
        #region Instance

        private static readonly SerializerSet instance;

        static SerializerSet()
        {
            instance = new SerializerSet();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                InitializeSerializers(assembly);

            AppDomain.CurrentDomain.AssemblyLoad += (s, args) => InitializeSerializers(args.LoadedAssembly);
        }

        private static void InitializeSerializers(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(ISerializer).IsAssignableFrom(type))
                {
                    var instanceField = type.GetField("Instance", BindingFlags.Public | BindingFlags.Static);
                    if (instanceField != null)
                    {
                        instanceField.GetValue(null);
                    }
                }
            }
        }

        public static SerializerSet Instance
        {
            get { return instance; }
        }

        #endregion

        private readonly ReaderWriterLockSlim serializersLock = new ReaderWriterLockSlim();
        private readonly Dictionary<string, ISerializer> serializers = new Dictionary<string, ISerializer>();

        internal SerializerSet()
        {
        }

        public void AddSerializer(Type type, ISerializer serializer)
        {
            serializersLock.EnterWriteLock();

            try
            {
                serializers.Add(type.FullName, serializer);
            }
            finally
            {
                serializersLock.ExitWriteLock();
            }
        }

        public ISerializer GetSerializer(Type type)
        {
            return GetSerializer(type.FullName);
        }

        private ISerializer GetSerializer(string typeName)
        {
            serializersLock.EnterReadLock();

            try
            {
                ISerializer serializer;
                serializers.TryGetValue(typeName, out serializer);
                return serializer;
            }
            finally
            {
                serializersLock.ExitReadLock();
            }
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
