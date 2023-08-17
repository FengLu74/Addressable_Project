using System;
using System.Diagnostics;
using UnityEngine;
namespace Kit {
    public static class AppUtils
    {
        /// <summary>
        /// 设置某个值
        /// </summary>
        /// <param name="name">路径名, 例如: "ns1.ns2.class.value"</param>
        /// <param name="value">要设置的值</param>
        /// <param name="value2">实际被设置的值</param>
        /// <returns>是否成功</returns>
        // ReSharper disable once UnusedMember.Global
        public static bool SetGlobalValue(string name, object value, out object value2)
        {
            var index = name.LastIndexOf('.');
            if (index > 0)
            {
                var cname = name.Substring(0, index);
                var vName = name.Substring(index + 1);
                var t = Type.GetType(cname);
                if (t != null)
                {
                    var info = t.GetField(vName);
                    if (info != null && info.IsStatic)
                    {
                        value2 = Convert.ChangeType(value, info.FieldType);
                        info.SetValue(null, value2);
                        return true;
                    }
                }
            }
            value2 = null;
            return false;
        }

        // ReSharper disable once UnusedMember.Global
        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                Log.LogError("Editor.OpenUrl: {0} / {1}", ex.ToString(), ex.Message);
            }
        }

        private static MonoBehaviour _main;
        public static void SetMainGameObject( MonoBehaviour value ) => _main = value;

        public static MonoBehaviour GetMainGameObject() => _main;
    }
}
