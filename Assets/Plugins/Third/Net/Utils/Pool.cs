using System;
using System.Collections.Generic;
namespace Net {
    public abstract class Pool {
        // ReSharper disable once UnusedMember.Global
        public static void ClearUpAllPool() {
            while (Pools.Count > 0) {
                Pools[0].ResetPool();
            }
        }
        // ReSharper disable once MemberCanBeProtected.Global
        public abstract void ResetPool();

        protected static readonly List<Pool> Pools = new List<Pool>();
        protected bool FreshPool { get; set; } = true;
    }
    // ReSharper disable once UnusedMember.Global
    public class Pool<T> : Pool where T : class, new() {
        public delegate T NewInstanceDelegate();
        public Pool() { }
        public Pool(NewInstanceDelegate newInstanceDelegate) =>
            _newInstanceDelegate = newInstanceDelegate;
        // ReSharper disable once UnusedMember.Global
        public int Count => _stackPool.Count;
        // ReSharper disable once UnusedMember.Global
        public T GetNew() {
            if (FreshPool) {
                Pools.Add(this);
                FreshPool = false;
            }
            if (_stackPool.Count == 0) {
                _stackPool.Push(NewInstance());
            }
            var t = _stackPool.Pop();
            var getNew = t as IGetNew;
            getNew?.OnGetNew();
            return t;
        }
        // ReSharper disable once UnusedMember.Global
        public void GiveBack(T t) {
            _stackPool.Push(t);
            var giveBack = t as IGiveBack;
            giveBack?.OnGiveBack();
        }
        public override void ResetPool() {
            _stackPool.Clear();
            FreshPool = true;
            Pools.Remove(this);
        }

        protected virtual T NewInstance() => _newInstanceDelegate != null
            ? _newInstanceDelegate.Invoke()
            : Activator.CreateInstance<T>();

        private readonly Stack<T> _stackPool = new Stack<T>(10);
        private readonly NewInstanceDelegate _newInstanceDelegate;
    }
}
