// automatically generated, do not modify

namespace FlatBuffers.Schema.Tests
{

using FlatBuffers;

public sealed class PingList : Table {
  public static PingList GetRootAsPingList(ByteBuffer _bb) { return GetRootAsPingList(_bb, new PingList()); }
  public static PingList GetRootAsPingList(ByteBuffer _bb, PingList obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public PingList __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Ticks { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public PingListItem GetItems(int j) { return GetItems(new PingListItem(), j); }
  public PingListItem GetItems(PingListItem obj, int j) { int o = __offset(6); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ItemsLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<PingList> CreatePingList(FlatBufferBuilder builder,
      int ticks = 0,
      VectorOffset itemsOffset = default(VectorOffset)) {
    builder.StartObject(2);
    PingList.AddItems(builder, itemsOffset);
    PingList.AddTicks(builder, ticks);
    return PingList.EndPingList(builder);
  }

  public static void StartPingList(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddTicks(FlatBufferBuilder builder, int ticks) { builder.AddInt(0, ticks, 0); }
  public static void AddItems(FlatBufferBuilder builder, VectorOffset itemsOffset) { builder.AddOffset(1, itemsOffset.Value, 0); }
  public static VectorOffset CreateItemsVector(FlatBufferBuilder builder, Offset<PingListItem>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartItemsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<PingList> EndPingList(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<PingList>(o);
  }
};


}
