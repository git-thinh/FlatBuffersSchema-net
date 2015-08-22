// automatically generated, do not modify

namespace FlatBuffers.Schema.Tests
{

using FlatBuffers;

public sealed class PingListItem : Table {
  public static PingListItem GetRootAsPingListItem(ByteBuffer _bb) { return GetRootAsPingListItem(_bb, new PingListItem()); }
  public static PingListItem GetRootAsPingListItem(ByteBuffer _bb, PingListItem obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public PingListItem __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Key { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Value { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<PingListItem> CreatePingListItem(FlatBufferBuilder builder,
      int key = 0,
      int value = 0) {
    builder.StartObject(2);
    PingListItem.AddValue(builder, value);
    PingListItem.AddKey(builder, key);
    return PingListItem.EndPingListItem(builder);
  }

  public static void StartPingListItem(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddKey(FlatBufferBuilder builder, int key) { builder.AddInt(0, key, 0); }
  public static void AddValue(FlatBufferBuilder builder, int value) { builder.AddInt(1, value, 0); }
  public static Offset<PingListItem> EndPingListItem(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<PingListItem>(o);
  }
};


}
