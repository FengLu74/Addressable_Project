namespace Net {
    public enum DbgLevel {
        Debug = 0,
        Info,
        Warning,
        Error,

        NoLog // 放在最后面，使用这个时表示不输出任何日志（!!!慎用!!!）
    }
}
