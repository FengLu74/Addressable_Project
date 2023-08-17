using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
namespace Net {
    public class PacketRecvTcp : PacketRecvBase {
        public PacketRecvTcp(ConnectionBase connection) : base(connection) =>
            _bytes = new byte[connection.RecvBufferSize];
        public override void Process() {
            if (Connection?.Socket == null) { return; }
            var tempWPos = Interlocked.Add(ref _wPos, 0);
            if (_rPos < tempWPos) {
                ProcessMessage(_bytes, _rPos, tempWPos - _rPos);
                Interlocked.Exchange(ref _rPos, tempWPos);
            } else if (tempWPos < _rPos) {
                // 回头
                ProcessMessage(_bytes, _rPos, _bytes.Length - _rPos);
                ProcessMessage(_bytes, 0, tempWPos);
                Interlocked.Exchange(ref _rPos, tempWPos);
            }
        }
        protected override void AsyncRecv() {
            if (Connection?.IsValid() != true) {
                Dbg.WarningMsg("PacketRecvTcp::AsyncRecv(): connection invalid!");
                return;
            }
            var socket = Connection.Socket;
            while (true) {
                var times = 0;
                var space = GetFreeSize();
                while (space == 0) {
                    if (times > 0) {
                        // 尝试1000次后断开连接
                        if (times > 1000) {
                            Dbg.ErrorMsg("PacketRecvTcp::AsyncRecv(): no space!");
                            Connection.Close(DisconnectReasonType.RecvNoSpace);
                            return;
                        }
                        if (ThreadHelper.Sleep()) { return; }
                    }
                    ++times;
                    space = GetFreeSize();
                }
                if (times > 0) {
                    // 出现一次即发警告，调整缓存大小
                    Dbg.WarningMsg(
                        $"PacketRecvTcp::AsyncRecv(): waiting for space, Please adjust 'PacketMax'! retries={times}");
                }

                int readLen;
                try {
                    readLen = socket.Receive(_bytes, _wPos, space, 0);
                } catch (SocketException e) {
                    Dbg.ErrorMsg(
                        $"PacketRecvTcp::AsyncRecv(): receive error, disconnect from '{socket.RemoteEndPoint}'! error = '{e}'");
                    Connection.Close(DisconnectReasonType.RecvFailed);
                    return;
                } catch (Exception e) {
                    Dbg.ErrorMsg(
                        $"PacketRecvTcp::AsyncRecv(): receive error, disconnect from '{socket.RemoteEndPoint}'! error = '{e}'");
                    Connection.Close(DisconnectReasonType.RecvFailed);
                    return;
                }
                if (readLen > 0) {
                    Interlocked.Add(ref _wPos, readLen);
                } else {
                    Dbg.WarningMsg(
                        $"PacketRecvTcp::AsyncRecv(): receive 0 bytes, disconnect from '{socket.RemoteEndPoint}'!");
                    Connection.Close(DisconnectReasonType.RecvZero);
                    return;
                }
            }
        }

        public void ProcessMessage(byte[] data, int offset, int len) {
            if (_packetRead == null) {
                GetNewPacket();
            }
            while (len > 0 && _expectSize > 0) {
                if (len < _expectSize) {
                    _writer.Write(data, offset, len);
                    _expectSize -= len;
                    break;
                }
                _writer.Write(data, offset, _expectSize);
                offset += _expectSize;
                len -= _expectSize;
                if (_packetRead.BodySize == 0) {
                    _packetRead.ReadHead(_buffer.GetData());
                    _expectSize = _packetRead.BodySize;
                    continue;
                }
                Connection.HandlePacket(_buffer);
                GiveBackPacket();
                GetNewPacket();
            }
        }

        private void GetNewPacket() {
            _packetRead = Connection.PacketReadFactory.GetNew();
            _buffer = Connection.BeginRecv();
            _writer = _buffer.BeginWrite();
            _expectSize = _packetRead.HeadSize;
        }
        private void GiveBackPacket() {
            Connection.PacketReadFactory.GiveBack(_packetRead);
            _packetRead = null;
            _buffer = null;
            _writer = null;
        }
        private int GetFreeSize() {
            var tempRPos = Interlocked.Add(ref _rPos, 0);
            if (_wPos == _bytes.Length) {
                if (tempRPos == 0) { return 0; }
                // 循环写
                Interlocked.Exchange(ref _wPos, 0);
            }
            if (tempRPos <= _wPos) {
                return _bytes.Length - _wPos;
            }
            return tempRPos - _wPos - 1;
        }

        private readonly byte[] _bytes;
        private int _wPos;
        private int _rPos;

        private PacketReadBase _packetRead;
        private Buffer _buffer;
        private BinaryWriter _writer;
        private int _expectSize;
    }
}
