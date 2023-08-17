using System;
using System.Threading;
namespace Net {
    public static class ThreadHelper {
        public static Thread CreateThread(ParameterizedThreadStart del) {
            var thread = new Thread(del);
            try {
                thread.CurrentCulture = EnglishUsCulture;
                thread.CurrentUICulture = EnglishUsCulture;
            } catch (Exception) {
                // ignored
            }
            return thread;
        }
        public static Thread CreateThread(ThreadStart del) {
            var thread = new Thread(del);
            try {
                thread.CurrentCulture = EnglishUsCulture;
                thread.CurrentUICulture = EnglishUsCulture;
            } catch (Exception) {
                // ignored
            }
            return thread;
        }
        public static bool Sleep() {
            try {
                Thread.Sleep(5);
            } catch (ThreadInterruptedException) {
                return true;
            }
            return false;
        }

        private static readonly System.Globalization.CultureInfo EnglishUsCulture =
            new System.Globalization.CultureInfo("en-US");
    }
}
