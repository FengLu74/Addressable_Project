using System;
using System.Net.Sockets;
using System.Threading;
namespace Net {
    public class PacketSendTcp : PacketSendBase {
        public PacketSendTcp(ConnectionBase connection) : base(connection) =>
            _bytes = new byte[Connection.SendBufferSize];
        public override bool Send(Buffer buffer) {
            var len = buffer.GetDataLength();
            if (len == 0) { return true; }
            Monitor.Enter(_sendingObj);
            if (!_sending) {
                if (_wPos == _rPos) {
                    _wPos = 0;
                    _rPos = 0;
                }
            }
            var tempRPos = _rPos;
            int space;
            var clampWPos = _wPos % _bytes.Length;
            var clampRPos = tempRPos % _bytes.Length;
            if (clampWPos >= clampRPos) {
                space = _bytes.Length - clampWPos + clampRPos - 1;
            } else {
                space = clampRPos - clampWPos - 1;
            }
            if (len > space) {
                // 发警告，调整缓存大小
                Dbg.ErrorMsg(
                    $"PacketSendTcp::Send(): no space, Please adjust 'PacketMax'! data({len}) > space({space}), wPos={_wPos}, rPos={tempRPos}");
                return false;
            }
            var expectTotal = clampWPos + len;
            if (expectTotal <= _bytes.Length) {
                Array.Copy(buffer.GetData(), 0, _bytes, clampWPos, len);
            } else {
                var remain = _bytes.Length - clampWPos;
                Array.Copy(buffer.GetData(), 0, _bytes, clampWPos, remain);
                // 回头
                Array.Copy(buffer.GetData(), remain, _bytes, 0, expectTotal - _bytes.Length);
            }
            _wPos += len;
            if (!_sending) {
                _sending = true;
                Monitor.Exit(_sendingObj);
                StartSend();
            } else {
                Monitor.Exit(_sendingObj);
            }
            return true;
        }
        protected override void AsyncSend() {
            if (Connection?.IsValid() != true) {
                Dbg.WarningMsg("PacketSendTcp::AsyncSend(): connection invalid!");
                return;
            }
            var socket = Connection.Socket;
            while (true) {
                Monitor.Enter(_sendingObj);
                var sendSize = _wPos - _rPos;
                var clampRPos = _rPos % _bytes.Length;
                if (clampRPos == 0) {
                    clampRPos = sendSize;
                }
                if (sendSize > _bytes.Length - clampRPos) {
                    sendSize = _bytes.Length - clampRPos;
                }
                int sendLen;
                try {
                    sendLen = socket.Send(_bytes, _rPos % _bytes.Length, sendSize, 0);
                } catch (SocketException e) {
                    Dbg.ErrorMsg(
                        $"PacketRecvTcp::AsyncSend(): send error, disconnect from '{socket.RemoteEndPoint}'! error = '{e}'");
                    Connection.Close(DisconnectReasonType.SendFailed);
                    Monitor.Exit(_sendingObj);
                    return;
                } catch (Exception e) {
                    Dbg.ErrorMsg(
                        $"PacketRecvTcp::AsyncSend(): send error, disconnect from '{socket.RemoteEndPoint}'! error = '{e}'");
                    Connection.Close(DisconnectReasonType.SendFailed);
                    Monitor.Exit(_sendingObj);
                    return;
                }
                _rPos += sendLen;
                if (_rPos == _wPos) {
                    // all done
                    _sending = false;
                    Monitor.Exit(_sendingObj);
                    return;
                }
                Monitor.Exit(_sendingObj);
            }
        }

        private readonly byte[] _bytes;
        private int _wPos;
        private int _rPos;

        private readonly object _sendingObj = new object();
        private bool _sending;
    }
}
