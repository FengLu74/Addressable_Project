using System;
using System.IO;
using Sirenix.OdinInspector;
namespace TCG.Editor {
    [Serializable]
    public class CommonBuildSettings {
        [LabelText("是否启用混淆")] public bool enableObfuscator = true;
        [LabelText("是否只编译工程，不打包资源")] public bool buildPlayerOnly;
        [LabelText("是否启用开发模式")] public bool isDevelopment;
        [LabelText("是否脚本调试")] public bool isScriptDebugging;
        [LabelText("是否清理编译")] public bool isClearBuild;
        [LabelText("数字版本号")] public int bundleVersionCode = 1;
        [LabelText("版本号")] public string bundleVersion = "1.0.0";
        [LabelText("产品名")] public string productName = "幻星牌";
        // ReSharper disable once StringLiteralTypo
        [LabelText("包名")] public string applicationIdentifier = "com.lattebank.ccg";
        [LabelText("导出目录")] [FolderPath] public string exportPath = "Build";
        [LabelText("是否自动连接Profiler")] public bool autoConnectProfiler;
        [LabelText("是否深度Profiler支持")] public bool deepProfilingSupport;

        public void ProcessClearBuild() {
            if (!isClearBuild) {
                if (!Directory.Exists(exportPath)) {
                    Directory.CreateDirectory(exportPath);
                }
                return;
            }
            var exportFullPath = Path.GetFullPath(exportPath);
            if (Directory.Exists(exportFullPath)) {
                Directory.Delete(exportFullPath, true);
            }
            Directory.CreateDirectory(exportFullPath);
        }
    }
}
