using UnityEngine;
using System.IO;

namespace Kit
{
    public class LogFile : ILog
    {
        private bool _init;
        private StreamWriter _sw;

        public void Init()
        {
            if (_init)
            {
                return;
            }

            _init = true;

            var logPath = DirectoryUtils.GetBaseFilePersistentPath("logs");

            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            var logFileName = logPath + "/gamelog";
            if (File.Exists(logFileName))
            {
                // ReSharper disable once StringLiteralTypo
                var bakFileName = logPath + "/gamelog_bak";
                if (File.Exists(bakFileName))
                {
                    File.Delete(bakFileName);
                }

                File.Move(logFileName, bakFileName);
                File.Delete(logFileName);
            }

            //var fs = new FileStream(logFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            //_sw = new StreamWriter(fs);
            var fs = new FileStream(logFileName, FileMode.Create);
            _sw = new StreamWriter(fs);
        }

        public void Shutdown()
        {
            if (null == _sw)
            {
                return;
            }

            _sw.Close();
            _sw = null;
        }

        public void LogMessage(string logContent, LogType logType)
        {
            if (null == _sw)
            {
                return;
            }

            _sw.WriteLine(logContent);
            _sw.Flush();
        }
    }
}