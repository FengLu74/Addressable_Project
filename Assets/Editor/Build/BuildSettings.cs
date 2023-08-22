using UnityEditor;
namespace TCG.Editor {
    /// <summary>
    /// 构建设置
    /// </summary>
    public class BuildSettings {
        public int BundleVersionCode = -99;

        public string ExportPath; //导出的工程目录，Gradle进行打包

        public string ProductName = "幻星牌";

        public string ExportName = "huanxing";

        public bool IsRelease;

        // ReSharper disable once StringLiteralTypo
        public string BundleIdentifier = "com.lattebank.ccg";

        public string BundleVersion = "0.0.0.0"; //版本号

        public string DefineSymbols;

        public ScriptingImplementation ScriptingBackend = ScriptingImplementation.IL2CPP;

        public int PackBundleFlag = (int)EnumAssetBundleType.All;

        public string IOSSdkVersion = "11.2";

        public AndroidSdkVersions AndroidAPILevel = AndroidSdkVersions.AndroidApiLevel28;

        public string ProfileID = "";

        public ProvisioningProfileType ProfileType = ProvisioningProfileType.Automatic;

        public string TeamID = "";

        public bool IsPackApk = true;

        public bool IsTest;

        public bool IsDevelopment;

        public bool BuildPlayerOnly;

        public bool IsCleanAllAb;

        public bool ScriptDebugging;

        public bool AutoConnectProfiler;

        public bool DeepProfilingSupport;

        public bool EnableObfuscator;
    }
}
