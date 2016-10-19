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

namespace FlatBuffers.Schema
{
    public interface ISerializer
    {
        byte[] Serialize(object obj);

        object Deserialize(byte[] data);
    }

    public interface ISerializer<TObject, TFlatBufferObject>
        where TFlatBufferObject : struct, IFlatbufferObject
    {
        Offset<TFlatBufferObject> Serialize(FlatBufferBuilder fbb, TObject obj);

        Offset<TFlatBufferObject>[] Serialize(FlatBufferBuilder fbb, TObject[] objects);

        TObject Deserialize(TFlatBufferObject? obj);

        TObject Deserialize(TFlatBufferObject obj);

        TObject[] Deserialize(int objectsLength, Func<int, TFlatBufferObject?> getObjects);
    }

    public abstract class Serializer<TObject, TFlatBufferObject> : ISerializer<TObject, TFlatBufferObject>, ISerializer
        where TFlatBufferObject : struct, IFlatbufferObject
    {
        public Serializer()
        {
            SerializerSet.Instance.AddSerializer(typeof(TObject), this);
        }

        public byte[] Serialize(object obj)
        {
            var fbb = new FlatBufferBuilder(1024);
            var offset = Serialize(fbb, (TObject)obj);
            fbb.Finish(offset.Value);

            return fbb.SizedByteArray();
        }

        public abstract Offset<TFlatBufferObject> Serialize(FlatBufferBuilder fbb, TObject obj);

        public Offset<TFlatBufferObject>[] Serialize(FlatBufferBuilder fbb, TObject[] objects)
        {
            if (objects == null)
                return null;

            var offsets = new Offset<TFlatBufferObject>[objects.Length];
            for (int i = 0; i < objects.Length; i++)
                offsets[i] = Serialize(fbb, objects[i]);

            return offsets;
        }

        public static StringOffset[] SerializeString(FlatBufferBuilder fbb, string[] objects)
        {
            if (objects == null)
                return null;

            var offsets = new StringOffset[objects.Length];
            for (int i = 0; i < objects.Length; i++)
                offsets[i] = fbb.CreateString(objects[i]);

            return offsets;
        }

        public object Deserialize(byte[] data)
        {
            if (data == null)
                return default(TObject);

            return Deserialize(GetRootAs(new ByteBuffer(data)));
        }

        protected abstract TFlatBufferObject GetRootAs(ByteBuffer buffer);

        public TObject Deserialize(TFlatBufferObject? obj)
        {
            if (!obj.HasValue)
                return default(TObject);

            return Deserialize(obj.Value);
        }

        public abstract TObject Deserialize(TFlatBufferObject obj);

        public TObject[] Deserialize(int objectsLength, Func<int, TFlatBufferObject?> getObjects)
        {
            if (objectsLength == 0)
                return null;

            var objects = new TObject[objectsLength];
            for (int i = 0; i < objectsLength; i++)
                objects[i] = Deserialize(getObjects(i));

            return objects;
        }

        public static T[] DeserializeScalar<T>(int objectsLength, Func<int, T> getObjects)
        {
            if (objectsLength == 0)
                return null;

            var objects = new T[objectsLength];
            for (int i = 0; i < objectsLength; i++)
                objects[i] = getObjects(i);

            return objects;
        }
    }
}
