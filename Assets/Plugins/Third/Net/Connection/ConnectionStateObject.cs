using System;
using System.Net.Sockets;

namespace Net {
    public class ConnectionStateObject {
        public string Ip { get; set; }
        public int Port { get; set; }
        public object UserData { get; set; }
        public string ParsedIp { get; set; }
        public DateTime ConnectToBeginTime { get; set; }
        public int ConnectTimeout { get; set; }
        public Socket Socket { get; set; }
        public ConnectionBase Connection { get; set; }
        public ConnectionBase.AsyncConnectMethod AsyncConnectMethod { get; set; }
        public ConnectionErrorType ConnectionErrorType { get; set; }
        public string ErrorMsg { get; set; }
    }
}
