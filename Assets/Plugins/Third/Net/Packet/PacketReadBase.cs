namespace Net {
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PacketReadBase : IGetNew {
        public virtual int HeadSize { get; } = 2;
        public virtual void OnGetNew() => BodySize = 0;
        // return total size
        protected virtual int OnReadHead(byte[] data) => (ushort)(data[0] | (uint)data[1] << 8);
        public int BodySize { get; protected set; }
        public virtual void ReadHead(byte[] data) => BodySize = OnReadHead(data) - HeadSize;
    }
}
