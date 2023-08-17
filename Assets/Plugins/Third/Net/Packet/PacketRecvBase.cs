using System;
namespace Net {
    public abstract class PacketRecvBase {
        public delegate void AsyncRecvMethod();

        public ConnectionBase Connection { get; }

        public PacketRecvBase(ConnectionBase connection) {
            Connection = connection;
            _asyncRecvMethod = AsyncRecv;
            _asyncCallback = AsyncRecvCallback;
        }

        public virtual void StartRecv() =>
            _asyncRecvMethod.BeginInvoke(_asyncCallback, _asyncRecvMethod);

        public abstract void Process();

        protected abstract void AsyncRecv();

        private static void AsyncRecvCallback(IAsyncResult ar) {
            try {
                var caller = (AsyncRecvMethod)ar.AsyncState;
                caller.EndInvoke(ar);
            } catch (ObjectDisposedException) { }
        }

        private readonly AsyncRecvMethod _asyncRecvMethod;
        private readonly AsyncCallback _asyncCallback;
    }
}
