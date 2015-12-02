// automatically generated, do not modify

namespace FlatBuffers.Schema.Tests
{

using FlatBuffers;

public sealed class PingMessage : Table {
  public static PingMessage GetRootAsPingMessage(ByteBuffer _bb) { return GetRootAsPingMessage(_bb, new PingMessage()); }
  public static PingMessage GetRootAsPingMessage(ByteBuffer _bb, PingMessage obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public PingMessage __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Count { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Msg { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public PingList GetLists(int j) { return GetLists(new PingList(), j); }
  public PingList GetLists(PingList obj, int j) { int o = __offset(8); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ListsLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<PingMessage> CreatePingMessage(FlatBufferBuilder builder,
      int count = 0,
      StringOffset msgOffset = default(StringOffset),
      VectorOffset listsOffset = default(VectorOffset)) {
    builder.StartObject(3);
    PingMessage.AddLists(builder, listsOffset);
    PingMessage.AddMsg(builder, msgOffset);
    PingMessage.AddCount(builder, count);
    return PingMessage.EndPingMessage(builder);
  }

  public static void StartPingMessage(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddCount(FlatBufferBuilder builder, int count) { builder.AddInt(0, count, 0); }
  public static void AddMsg(FlatBufferBuilder builder, StringOffset msgOffset) { builder.AddOffset(1, msgOffset.Value, 0); }
  public static void AddLists(FlatBufferBuilder builder, VectorOffset listsOffset) { builder.AddOffset(2, listsOffset.Value, 0); }
  public static VectorOffset CreateListsVector(FlatBufferBuilder builder, Offset<PingList>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartListsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<PingMessage> EndPingMessage(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<PingMessage>(o);
  }
  public static void FinishPingMessageBuffer(FlatBufferBuilder builder, Offset<PingMessage> offset) { builder.Finish(offset.Value); }
};


}
