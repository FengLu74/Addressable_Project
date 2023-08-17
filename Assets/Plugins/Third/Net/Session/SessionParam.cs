namespace Net {
    public class SessionParam {
        public int ConnectTimeout { get; set; } = 30000; // 毫秒
        public int TcpSendBufferSize { get; set; } = 1460;
        public int TcpRecvBufferSize { get; set; } = 1460;
        public int UdpSendBufferSize { get; set; } = 128;
        public int UdpRecvBufferSize { get; set; } = 128;
        public bool DisableUdp { get; set; }
        public string UdpHello { get; set; } = "Hello";
        public string UdpHelloAck { get; set; } = "Ack";
    }
}
