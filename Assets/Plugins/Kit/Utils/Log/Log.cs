#define ENABLE_LOG //是否显示Log，包括在真机上通过log-view插件显示Log
using UnityEngine;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace Kit
{
    public interface ILog
    {
        // ReSharper disable once UnusedMemberInSuper.Global
        void Init();
        void LogMessage(string logContent, LogType logType);
        void Shutdown();
    }

    public static class Log
    {
        #region info/wran/error 日志

        // ReSharper disable once UnusedMember.Local
        //private static readonly object InfoLock = new object();
        private static readonly object WarnLock = new object();
        private static readonly object ErrLock = new object();
        private static readonly object ExceptionLock = new object();
        private static readonly HashSet<string> Errs = new HashSet<string>();
        private static readonly HashSet<string> Exceptions = new HashSet<string>();

        private static bool _showLog;

        private static readonly List<ILog> Logs = new List<ILog>();

        public static void Init(bool showLog, bool log2File)
        {
            _showLog = showLog;
            RegisterApplicationLogListener();

            // ReSharper disable once InvertIf
            if (log2File)
            {
                var logFile = new LogFile();
                logFile.Init();
                Logs.Add(logFile);
            }
        }

        public static void Shutdown()
        {
            UnregisterApplicationLogListener();

            foreach (var log in Logs)
            {
                log?.Shutdown();
            }
        }

        public static void RegisterLogListen(ILog log) => Logs.Add(log);

        public static void UnregisterLogListen(ILog log) => Logs.Remove(log);

        // ReSharper disable once UnusedMember.Global
        public static bool LogInfoEnable() => _showLog;

        // ReSharper disable once MemberCanBePrivate.Global
        public static bool LogDebugEnable()
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }

        public static void LogDebug(string fmt, params object[] args)
        {
            if (!LogDebugEnable())
            {
                return;
            }

            var str = args.Length == 0 ? fmt : string.Format(fmt, args);
            Debug.Log(str);
        }

        // 普通信息输出
        public static void LogInfo(string fmt, params object[] args) => LogInfo(fmt, null, args);

        public static void LogInfo(string fmt, object context, params object[] args)
        {
            if (!LogInfoEnable())
            {
                return;
            }

            //lock (InfoLock)
            {
                var str = args.Length == 0 ? fmt : string.Format(fmt, args);
                var frame = Time.frameCount;
                var log = $"frame:{frame},{str}";
                Debug.Log(log, context as Object);
            }
        }

        // 输出 警告
        public static void LogWarning(string fmt, params object[] args) => LogWarning(fmt, null, args);

        public static void LogWarning(string fmt, object context, params object[] args)
        {
            if (!LogInfoEnable())
            {
                return;
            }

            lock (WarnLock)
            {
                var str = args.Length == 0 ? fmt : string.Format(fmt, args);
                Debug.LogWarning(str, context as Object);
            }
        }

        // 输出错误
        public static void LogError(string fmt, params object[] args) => LogError(fmt, null, args);

        public static void LogError(string fmt, object context, params object[] args)
        {
            var str = args.Length == 0 ? fmt : string.Format(fmt, args);
            lock (ErrLock)
            {
                if (Errs.Contains(str))
                {
                    return;
                }

                Errs.Add(str);
                Debug.LogError(str, context as Object);
            }
        }

        // 输出错误
        public static void LogException(System.Exception e) => LogException(e, null);

        public static void LogException(System.Exception e, object context)
        {
            var str = e.ToString();
            lock (ExceptionLock)
            {
                if (Exceptions.Contains(str))
                {
                    return;
                }

                Exceptions.Add(str);
                Debug.LogException(e, context as Object);
            }
        }

        #endregion

        private static void RegisterApplicationLogListener() => Application.logMessageReceivedThreaded += LogHandler;

        private static void UnregisterApplicationLogListener() => Application.logMessageReceivedThreaded -= LogHandler;

        private static void LogHandler(string strLogInfo, string stackTrace, LogType logType)
        {
            if (Logs.Count == 0)
            {
                return;
            }

            var now = TimeUtils.GetServerCurTime();
            var str = $"{now.ToLongTimeString()}({now.Millisecond}):";
            var error = logType == LogType.Error || logType == LogType.Exception;
            string logContent;
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (error)
            {
                logContent = $"<color=red>[{logType}] Time:{str} {strLogInfo} \n {stackTrace}</color>";
            }
            else
            {
                logContent = $"[{logType}] Time:{str} {strLogInfo}";
            }

            foreach (var log in Logs)
            {
                log?.LogMessage(logContent, logType);
            }
        }
    }
}
