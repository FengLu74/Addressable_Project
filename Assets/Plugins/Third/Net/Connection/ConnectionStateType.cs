namespace Net {
    public enum ConnectionStateType {
        None,
        GetHostEntry,
        GetHostEntryTimeout,
        GetHostEntryFailed,
        Connecting,
        ConnectTimeout,
        ConnectFailed,
        Connected
    }
}
