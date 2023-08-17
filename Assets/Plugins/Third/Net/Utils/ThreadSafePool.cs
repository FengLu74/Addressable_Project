using System;
using System.Collections.Generic;
using System.Threading;
namespace Net {
    public abstract class ThreadSafePool {
        public static void ClearAll() {
            lock (Pools) {
                foreach (var pool in Pools) {
                    pool.Clear();
                }
                Pools.Clear();
            }
        }
        public static void Remove(ThreadSafePool pool) {
            lock (Pools) {
                pool.Clear();
                Pools.Remove(pool);
            }
        }
        public abstract void Clear();

        protected static readonly List<ThreadSafePool> Pools = new List<ThreadSafePool>();
        protected int Fresh = 1;
    }
    public sealed class ThreadSafePool<T> : ThreadSafePool {
        public int Count {
            get {
                lock (_stack) {
                    return _stack.Count;
                }
            }
        }
        public T GetNew() {
            var fresh = Interlocked.Add(ref Fresh, 0);
            if (fresh == 1) {
                lock (Pools) {
                    Pools.Add(this);
                }
                Interlocked.Exchange(ref Fresh, 0);
            }
            T t;
            lock (_stack) {
                if (_stack.Count == 0) {
                    _stack.Push(NewInstance());
                }
                t = _stack.Pop();
            }
            var getNew = t as IGetNew;
            getNew?.OnGetNew();
            return t;
        }
        public void GiveBack(T t) {
            lock (_stack) {
                _stack.Push(t);
            }
            var giveBack = t as IGiveBack;
            giveBack?.OnGiveBack();
        }
        public override void Clear() {
            lock (_stack) {
                _stack.Clear();
            }
            Interlocked.Exchange(ref Fresh, 1);
        }

        private static T NewInstance() => Activator.CreateInstance<T>();

        private readonly Stack<T> _stack = new Stack<T>(10);
    }
}
