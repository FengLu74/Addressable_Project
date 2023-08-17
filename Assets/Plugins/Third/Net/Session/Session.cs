using System;
using System.Net;
namespace Net {
    public class Session {
        public SessionParam SessionParam { get; } = new SessionParam();

        public Action<ConnectResult> OnConnectionResult { get; set; }
        public Action<DisconnectReason> OnDisconnect { get; set; }
        public Action<Buffer> OnReceivedBuffer { get; set; }

        public IPEndPoint LocalEndPoint => (IPEndPoint)_connectionTcp.Socket?.LocalEndPoint;

        public void Init() {
            _connectionTcp = new ConnectionTcp();
            InitConnection(_connectionTcp);
            if (SessionParam.DisableUdp) {
                _connection = _connectionTcp;
            }
            // else {
            //     _connectionKcp = new ConnectionKcp();
            //     InitConnection(_connectionKcp);
            //     _connection = _connectionKcp;
            // }
        }
        public void RegisterPacketRead<T>() where T : PacketReadBase =>
            _connectionTcp.RegisterPacketRead<T>();
        public void ConnectTo(string ip, int port, object userData) {
            UnregisterConnection(_connection);
            if (SessionParam.DisableUdp) {
                _connection = _connectionTcp;
            }
            // else {
            //     _connection = _connectionKcp;
            // }
            RegisterConnection(_connection);
            _connection.ConnectTo(ip, port, userData);
        }
        public bool Send(byte[] data) => _connection.Send(data);
        public bool Send(byte[] data, int offset, int len) => _connection.Send(data, offset, len);
        public void Close() {
            if (_connection == null) { return; }
            _connection.Close(DisconnectReasonType.Manually);
            ProcessMainThread();
            UnregisterConnection(_connection);
            _connection = null;
        }
        public void ProcessMainThread() => _connection?.ProcessMainThread();
        public void Process() => _connection?.Process();

        // ReSharper disable once ArrangeMethodOrOperatorBody
        private void OnConnectionResultCallback(ConnectResult connectResult) {
            // ReSharper disable once ArrangeMethodOrOperatorBody
            OnConnectionResult?.Invoke(connectResult);
            // if (connectResult.Success) { return; }
            // if (!(_connection is ConnectionKcp)) { return; }
            // UnregisterConnection(_connection);
            // _connection = _connectionTcp;
            // RegisterConnection(_connection);
            // _connection.ConnectTo(connectResult.Ip, connectResult.Port, connectResult.UserData);
        }

        private void InitConnection(ConnectionBase connection) {
            connection.ConnectTimeout = SessionParam.ConnectTimeout;
            connection.RecvBufferSize = SessionParam.TcpRecvBufferSize;
            connection.SendBufferSize = SessionParam.TcpSendBufferSize;
            // if (connection is ConnectionKcp connectionKcp) {
            //     connectionKcp.UdpHello = SessionParam.UdpHello;
            //     connectionKcp.UdpHelloAck = SessionParam.UdpHelloAck;
            // }
            connection.Init();
        }
        private static void UnregisterConnection(ConnectionBase connection) {
            if (connection == null) { return; }
            connection.OnConnectionResult = null;
            connection.OnDisconnect = null;
            connection.OnRecv = null;
            connection.Reset();
        }
        private void RegisterConnection(ConnectionBase connection) {
            if (connection == null) { return; }
            connection.OnConnectionResult = OnConnectionResultCallback;
            connection.OnDisconnect = OnDisconnect;
            connection.OnRecv = OnReceivedBuffer;
        }

        // private ConnectionKcp _connectionKcp;
        private ConnectionTcp _connectionTcp;
        private ConnectionBase _connection;
    }
}
