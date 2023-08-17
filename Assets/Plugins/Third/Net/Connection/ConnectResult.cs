namespace Net {
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ConnectResult {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string Ip { get; set; }
        public int Port { get; set; }
        public bool Success { get; set; }
        public object UserData { get; set; }
        public ConnectionErrorType ConnectionErrorType { get; set; }
        public string ErrorMsg { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }
}
