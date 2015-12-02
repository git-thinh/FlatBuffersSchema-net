// automatically generated, do not modify

namespace FlatBuffers.Schema
{

using FlatBuffers;

public sealed class ProtocolMessage : Table {
  public static ProtocolMessage GetRootAsProtocolMessage(ByteBuffer _bb) { return GetRootAsProtocolMessage(_bb, new ProtocolMessage()); }
  public static ProtocolMessage GetRootAsProtocolMessage(ByteBuffer _bb, ProtocolMessage obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ProtocolMessage __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public byte GetBody(int j) { int o = __offset(6); return o != 0 ? bb.Get(__vector(o) + j * 1) : (byte)0; }
  public int BodyLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ProtocolMessage> CreateProtocolMessage(FlatBufferBuilder builder,
      int id = 0,
      VectorOffset bodyOffset = default(VectorOffset)) {
    builder.StartObject(2);
    ProtocolMessage.AddBody(builder, bodyOffset);
    ProtocolMessage.AddId(builder, id);
    return ProtocolMessage.EndProtocolMessage(builder);
  }

  public static void StartProtocolMessage(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddBody(FlatBufferBuilder builder, VectorOffset bodyOffset) { builder.AddOffset(1, bodyOffset.Value, 0); }
  public static VectorOffset CreateBodyVector(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); for (int i = data.Length - 1; i >= 0; i--) builder.AddByte(data[i]); return builder.EndVector(); }
  public static void StartBodyVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(1, numElems, 1); }
  public static Offset<ProtocolMessage> EndProtocolMessage(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ProtocolMessage>(o);
  }
  public static void FinishProtocolMessageBuffer(FlatBufferBuilder builder, Offset<ProtocolMessage> offset) { builder.Finish(offset.Value); }
};


}
