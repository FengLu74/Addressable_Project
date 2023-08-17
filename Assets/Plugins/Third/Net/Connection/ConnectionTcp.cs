using System;
using System.Net.Sockets;
namespace Net {
    public class ConnectionTcp : ConnectionBase {
        public override uint PacketMax { get; } = 1460;
        public override int SendBufferSize { get; set; } = 1460;
        public override int RecvBufferSize { get; set; } = 1460;
        protected override Socket CreateSocket() {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer,
                RecvBufferSize * 2);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer,
                SendBufferSize * 2);
            socket.NoDelay = true;
            return socket;
        }
        protected override PacketRecvBase CreatePacketRecv() =>
            new PacketRecvTcp(this);
        protected override PacketSendBase CreatePacketSend() =>
            new PacketSendTcp(this);
        protected override void OnAsyncConnect(ConnectionStateObject state) {
            try {
                state.Socket.Connect(state.ParsedIp, state.Port);
            } catch (Exception e) {
                Dbg.ErrorMsg(
                    $"ConnectionTcp::OnAsyncConnect(), connect to '{state.ParsedIp}:{state.Port}' fault! error = '{e}'");
                state.ConnectionErrorType = ConnectionErrorType.ConnectFailed;
                state.ErrorMsg = e.ToString();
            }
        }
    }
}
