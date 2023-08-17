using System.Text;
namespace Kit {
    public static partial class Extensions {
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        public static bool HasUpperCase(this string str) {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < str.Length; ++i) {
                if (char.IsUpper(str[i])) {
                    return true;
                }
            }
            return false;
        }

        public static void AppendEx(this StringBuilder stringBuilder, uint arg) {
            if (arg == 0) {
                stringBuilder.Append('0');
                return;
            }

            var len = stringBuilder.Length;
            while (arg > 0) {
                var next = arg / 10;
                var ch = (char)('0' + (arg - next * 10));
                stringBuilder.Insert(len, ch);
                arg = next;
            }
        }

        public static void AppendEx(this StringBuilder stringBuilder, int arg) {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (arg == 0) {
                stringBuilder.Append('0');
                return;
            }
            if (arg < 0) {
                arg = -arg;
                stringBuilder.Append('-');
            }
            stringBuilder.AppendEx((uint)arg);
        }
    }
}
