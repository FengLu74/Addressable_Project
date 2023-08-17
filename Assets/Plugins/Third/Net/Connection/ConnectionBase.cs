using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
namespace Net {
    public abstract class ConnectionBase {
        public delegate void AsyncConnectMethod(ConnectionStateObject state);

        public Action<ConnectResult> OnConnectionResult { private get; set; }
        public Action<DisconnectReason> OnDisconnect { private get; set; }
        public Action<Buffer> OnRecv { private get; set; }

        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract uint PacketMax { get; }
        public abstract int SendBufferSize { get; set; }
        public abstract int RecvBufferSize { get; set; }
        public void RegisterPacketRead<T>() where T : PacketReadBase =>
            PacketReadFactory = new PacketReadFactory<T>();
        public PacketReadFactory PacketReadFactory {
            get {
                if (_packetReadFactory == null) {
                    RegisterPacketRead<PacketReadBase>();
                }
                return _packetReadFactory;
            }
            private set => _packetReadFactory = value;
        }

        public int ConnectTimeout { private get; set; } = 30000; // 毫秒

        public Socket Socket { get; private set; }
        public virtual EndPoint RemoteEndPoint => Socket.RemoteEndPoint;
        protected PacketRecvBase PacketRecv { get; private set; }
        protected PacketSendBase PacketSend { get; set; }

        private ConnectionStateType ConnectionStateType { get; set; }

        public bool IsServer { get; private set; }

        public void Init() {
            _mainThreadEvent.Callback = OnResultEvent;
            Reset();
        }
        ~ConnectionBase() {
            Dbg.DebugMsg("ConnectionBase destructed");
            Reset();
        }
        public virtual void Reset() {
            ConnectionStateType = ConnectionStateType.None;
            PacketRecv = null;
            PacketSend = null;
            _mainThreadEvent.Reset();
            if (Socket == null) { return; }
            try {
                var socketRemoteEndPoint = Socket.RemoteEndPoint;
                if (socketRemoteEndPoint != null) {
                    Dbg.DebugMsg(
                        $"ConnectionBase::reset, close socket from '{socketRemoteEndPoint}'");
                }
            } catch (Exception) {
                // ignored
            }
            CloseSocket();
        }
        public void ConnectTo(string ip, int port, object userData) {
            if (IsValid()) {
                throw new InvalidOperationException("Have already connected!");
            }
            ConnectionStateType = ConnectionStateType.None;
            var state = new ConnectionStateObject {
                Ip = ip,
                Port = port,
                UserData = userData,
                Socket = Socket,
                Connection = this,
                ConnectToBeginTime = DateTime.Now,
                ConnectTimeout = ConnectTimeout
            };
            // Events.RegisterIn<ConnectionStateObject>((uint)InEvents.ConnectionState, OnConnectionStateEvent);
            if (!Regex.IsMatch(ip)) {
                GetHostEntry(state);
            } else {
                state.ParsedIp = ip;
                Connect(state);
            }
        }
        public virtual void StartServer(Socket socket, IPEndPoint remote, uint connId) {
            _mainThreadEvent.Callback = OnResultEvent;

            lock (_socketLock) {
                Socket = socket;
            }
            IsServer = true;
            ConnectionStateType = ConnectionStateType.Connected;

            PacketRecv = CreatePacketRecv();
            PacketRecv.StartRecv();
        }
        public bool Send(byte[] data) => Send(data, 0, data.Length);
        public virtual bool Send(byte[] data, int offset, int len) {
            if (!IsValid()) {
                throw new ArgumentException("Invalid socket!");
            }
            if (PacketSend == null) {
                PacketSend = CreatePacketSend();
            }
            var buffer = BufferPool.GetNew();
            buffer.WriteData(data, offset, len);
            // TODO: filter send

            var ret = PacketSend.Send(buffer);
            BufferPool.GiveBack(buffer);
            return ret;
        }
        public Buffer BeginRecv() => BufferPool.GetNew();
        private void EndRecv(Buffer buffer) => BufferPool.GiveBack(buffer);
        public void ProcessMainThread() => _mainThreadEvent.Process();
        public virtual void Process() {
            if (!IsValid()) { return; }
            PacketRecv?.Process();
        }
        public virtual void Close(DisconnectReasonType disconnectReasonType) {
            if (Socket != null) {
                if (disconnectReasonType != DisconnectReasonType.Manually) {
                    PacketRecv?.Process();
                }
                AddEvent(disconnectReasonType);
                CloseSocket();
            }
            ConnectionStateType = ConnectionStateType.None;
        }
        public virtual bool IsValid() => Socket?.Connected == true;

        internal void HandlePacket(Buffer buffer) => AddEvent(buffer);

        protected abstract Socket CreateSocket();
        protected abstract PacketRecvBase CreatePacketRecv();
        protected abstract PacketSendBase CreatePacketSend();
        protected abstract void OnAsyncConnect(ConnectionStateObject state);
        protected virtual void OnAsyncConnectCallback(ConnectionStateObject state) { }

        private void CloseSocket() {
            if (IsServer && Socket.ProtocolType == ProtocolType.Udp) { return; }
            lock (_socketLock) {
                Socket.Close(0);
                Socket = null;
            }
        }

        private void AsyncGetHostEntry(ConnectionStateObject state) {
            Dbg.DebugMsg(
                $"ConnectionBase::AsyncConnect, will get host entry '{state.Ip}' ...");
            try {
                var ipHost = Dns.GetHostEntry(state.Ip);
                state.ParsedIp = ipHost.AddressList[0].ToString();
            } catch (Exception e) {
                Dbg.ErrorMsg(
                    $"ConnectionBase::AsyncGetHostEntry, '{state.Ip}' fault! error = '{e}'");
                ConnectionStateType = ConnectionStateType.GetHostEntryFailed;
                state.ConnectionErrorType = ConnectionErrorType.GetHostEntryFailed;
                state.ErrorMsg = e.ToString();
            }
        }
        private void AsyncGetHostEntryTimeout(object arObj) {
            var ar = (IAsyncResult)arObj;
            var state = (ConnectionStateObject)ar.AsyncState;
            var elapsed = (int)(DateTime.Now - state.ConnectToBeginTime).TotalMilliseconds;
            var timeout = state.ConnectTimeout - elapsed;
            if (ar.AsyncWaitHandle.WaitOne(timeout, true)) {
                Dbg.DebugMsg("AsyncWaitHandle safe exit");
                return;
            }
            // 超时
            ConnectionStateType = ConnectionStateType.GetHostEntryTimeout;
            state.ConnectionErrorType = ConnectionErrorType.GetHostEntryTimeout;
            state.ErrorMsg = $"{state.ConnectionErrorType}";
            AddEvent(state);
        }
        private void AsyncGetHostEntryCallback(IAsyncResult ar) {
            var state = (ConnectionStateObject)ar.AsyncState;
            Dbg.DebugMsg(
                $"ConnectionBase::AsyncConnectCB, connect to '{state.Ip}:{state.Port}' finish. error = '{state.ErrorMsg}'");
            state.AsyncConnectMethod.EndInvoke(ar);
            if (state.ConnectionErrorType != ConnectionErrorType.None) {
                AddEvent(state);
            } else {
                Connect(state);
            }
        }
        private void GetHostEntry(ConnectionStateObject state) {
            Dbg.DebugMsg($"get host entry {state.Ip}...");
            ConnectionStateType = ConnectionStateType.GetHostEntry;
            state.AsyncConnectMethod = AsyncGetHostEntry;
            var ar = state.AsyncConnectMethod.BeginInvoke(state,
                AsyncGetHostEntryCallback, state);
            var th = ThreadHelper.CreateThread(AsyncGetHostEntryTimeout);
            th.Start(ar);
        }
        private void Connect(ConnectionStateObject state) {
            Dbg.DebugMsg($"connect to {state.Ip}:{state.Port}...");
            ConnectionStateType = ConnectionStateType.Connecting;
            lock (_socketLock) {
                Socket = CreateSocket();
                state.Socket = Socket;
            }
            state.AsyncConnectMethod = AsyncConnect;
            var ar = state.AsyncConnectMethod.BeginInvoke(state, AsyncConnectCallback, state);
            var th = ThreadHelper.CreateThread(AsyncConnectTimeout);
            th.Start(ar);
        }
        private void AsyncConnect(ConnectionStateObject state) {
            Dbg.DebugMsg(
                $"ConnectionBase::AsyncConnect, will connect to '{state.Ip}:{state.Port}' ...");
            OnAsyncConnect(state);
        }
        private void AsyncConnectTimeout(object arObj) {
            var ar = (IAsyncResult)arObj;
            var state = (ConnectionStateObject)ar.AsyncState;
            var elapsed = (int)(DateTime.Now - state.ConnectToBeginTime).TotalMilliseconds;
            var timeout = state.ConnectTimeout - elapsed;
            if (ar.AsyncWaitHandle.WaitOne(timeout, true)) {
                Dbg.DebugMsg("AsyncWaitHandle safe exit");
                return;
            }
            // 超时
            ConnectionStateType = ConnectionStateType.ConnectTimeout;
            state.ConnectionErrorType = ConnectionErrorType.ConnectTimeout;
            state.ErrorMsg = $"{state.ConnectionErrorType}";
            AddEvent(state);
        }
        private void AsyncConnectCallback(IAsyncResult ar) {
            var state = (ConnectionStateObject)ar.AsyncState;
            OnAsyncConnectCallback(state);
            Dbg.DebugMsg(
                $"ConnectionBase::AsyncConnectCB, connect to '{state.Ip}:{state.Port}' finish. error = '{state.ErrorMsg}'");
            state.AsyncConnectMethod.EndInvoke(ar);
            ConnectionStateType = state.ConnectionErrorType != ConnectionErrorType.None
                ? ConnectionStateType.ConnectFailed
                : ConnectionStateType.Connected;
            AddEvent(state);
            if (ConnectionStateType != ConnectionStateType.Connected) { return; }
            PacketRecv = CreatePacketRecv();
            PacketRecv.StartRecv();
        }
        private void AddEvent(DisconnectReasonType disconnectReasonType) {
            var disconnectReason = _disconnectReasonPool.GetNew();
            disconnectReason.DisconnectReasonType = disconnectReasonType;
            disconnectReason.Remote = (IPEndPoint)RemoteEndPoint;
            _mainThreadEvent.AddEvent((uint)ConnectionEventType.Disconnect, disconnectReason);
        }
        private void AddEvent(Buffer recvBuffer) =>
            _mainThreadEvent.AddEvent((uint)ConnectionEventType.RecvPacket, recvBuffer);
        private void AddEvent(ConnectionStateObject state) {
            var connectResult = _connectResultPool.GetNew();
            connectResult.Ip = state.Ip;
            connectResult.Port = state.Port;
            connectResult.Success = state.ConnectionErrorType == ConnectionErrorType.None;
            connectResult.UserData = state.UserData;
            connectResult.ConnectionErrorType = state.ConnectionErrorType;
            connectResult.ErrorMsg = state.ErrorMsg;
            _mainThreadEvent.AddEvent((uint)ConnectionEventType.ConnectResult, connectResult);
        }
        private void OnResultEvent(uint id, object data) {
            var connectionEventType = (ConnectionEventType)id;
            switch (connectionEventType) {
            case ConnectionEventType.None:
                throw new ArgumentOutOfRangeException();
            case ConnectionEventType.ConnectResult:
                var connectResult = (ConnectResult)data;
                OnConnectionResult?.Invoke(connectResult);
                _connectResultPool.GiveBack(connectResult);
                break;
            case ConnectionEventType.RecvPacket:
                var buffer = (Buffer)data;
                OnRecv?.Invoke(buffer);
                EndRecv(buffer);
                break;
            case ConnectionEventType.Disconnect:
                var disconnectReason = (DisconnectReason)data;
                OnDisconnect?.Invoke(disconnectReason);
                _disconnectReasonPool.GiveBack(disconnectReason);
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        private static readonly Regex Regex =
            new Regex(
                @"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))");

        private readonly ThreadSafePool<ConnectResult> _connectResultPool =
            new ThreadSafePool<ConnectResult>();
        private readonly ThreadSafePool<DisconnectReason>
            _disconnectReasonPool = new ThreadSafePool<DisconnectReason>();
        protected ThreadSafePool<Buffer> BufferPool { get; } = new ThreadSafePool<Buffer>();
        private readonly MainThreadEvent _mainThreadEvent = new MainThreadEvent();
        private PacketReadFactory _packetReadFactory;

        private readonly object _socketLock = new object();
    }
}
