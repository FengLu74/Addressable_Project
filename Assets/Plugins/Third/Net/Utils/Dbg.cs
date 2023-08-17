using System;
using System.Collections.Generic;
namespace Net {
    public static class Dbg {
        public static DbgLevel DbgLevel { get; set; } = DbgLevel.Warning;
        public static Action<string> LogDebugDelegate { private get; set; }
        public static Action<string> LogInfoDelegate { private get; set; }
        public static Action<string> LogWarningDelegate { private get; set; }
        public static Action<string> LogErrorDelegate { private get; set; }

        public static void InfoMsg(string s) {
            if (DbgLevel.Info >= DbgLevel) {
                LogInfoDelegate?.Invoke(FormatMessage(DbgLevel.Info, s));
            }
        }
        public static void DebugMsg(string s) {
            if (DbgLevel.Debug >= DbgLevel) {
                LogDebugDelegate?.Invoke(FormatMessage(DbgLevel.Debug, s));
            }
        }
        public static void WarningMsg(string s) {
            if (DbgLevel.Warning >= DbgLevel) {
                LogWarningDelegate?.Invoke(FormatMessage(DbgLevel.Warning, s));
            }
        }
        public static void ErrorMsg(string s) {
            if (DbgLevel.Error >= DbgLevel) {
                LogErrorDelegate?.Invoke(FormatMessage(DbgLevel.Error, s));
            }
        }
        public static void ProfileStart(string name) {
            if (!Profiles.TryGetValue(name, out var p)) {
                p = new Profile(name);
                Profiles.Add(name, p);
            }
            p.Start();
        }
        public static void ProfileEnd(string name) => Profiles[name].Stop();

        private static string GetLogTypeStr(DbgLevel logType) {
            switch (logType) {
            case DbgLevel.Debug:
                return "Debug";
            case DbgLevel.Info:
                return "Info";
            case DbgLevel.Warning:
                return "Warning";
            case DbgLevel.Error:
                return "Error";
            case DbgLevel.NoLog:
                throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            default:
                throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }
        private static string FormatMessage(DbgLevel logType, string message) =>
            $"[{GetLogTypeStr(logType)}] [{DateTime.Now.ToString(TimeFormat)}]: {message}";

        private static readonly Dictionary<string, Profile> Profiles =
            new Dictionary<string, Profile>();
        private const string TimeFormat = "yyyy-MM-dd HH:mm:ss fff";
    }
}
