namespace Net {
    public abstract class PacketReadFactory {
        public abstract PacketReadBase GetNew();
        public abstract void GiveBack(PacketReadBase t);
    }
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PacketReadFactory<T> : PacketReadFactory where T : PacketReadBase {
        public override PacketReadBase GetNew() => _pool.GetNew();
        public override void GiveBack(PacketReadBase t) => _pool.GiveBack((T)t);
        private readonly ThreadSafePool<T> _pool = new ThreadSafePool<T>();
    }
}
