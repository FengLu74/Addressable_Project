using System;
namespace Net {
    public abstract class PacketSendBase {
        public delegate void AsyncSendMethod();

        public ConnectionBase Connection { get; }

        public PacketSendBase(ConnectionBase connection) {
            Connection = connection;
            _asyncSendMethod = AsyncSend;
            _asyncCallback = AsyncSendCallback;
        }

        public abstract bool Send(Buffer buffer);

        protected void StartSend() => _asyncSendMethod.BeginInvoke(_asyncCallback, _asyncSendMethod);

        protected abstract void AsyncSend();

        private static void AsyncSendCallback(IAsyncResult ar) {
            var caller = (AsyncSendMethod)ar.AsyncState;
            try {
                caller.EndInvoke(ar);
            } catch (ObjectDisposedException) { }
        }

        private readonly AsyncSendMethod _asyncSendMethod;
        private readonly AsyncCallback _asyncCallback;
    }
}
