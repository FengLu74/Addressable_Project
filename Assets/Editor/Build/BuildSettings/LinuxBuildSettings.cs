using System;
using Sirenix.OdinInspector;
using UnityEditor;
namespace TCG.Editor {
    [Serializable]
    public class LinuxBuildSettings {
        [HideLabel, BoxGroup] public CommonBuildSettings commonBuildSettings;
        [LabelText("脚本后端")]
        [EnumPaging]
        public ScriptingImplementation scriptingImplementation = ScriptingImplementation.IL2CPP;
        [Button(ButtonSizes.Large)]
        private void Build() {
            commonBuildSettings.ProcessClearBuild();
            var buildSettings = new BuildSettings {
                BundleVersionCode = commonBuildSettings.bundleVersionCode,
                ExportPath = commonBuildSettings.exportPath,
                ProductName = commonBuildSettings.productName,
                BundleIdentifier = commonBuildSettings.applicationIdentifier,
                BundleVersion = commonBuildSettings.bundleVersion,
                ScriptingBackend = scriptingImplementation,
                IsDevelopment = commonBuildSettings.isDevelopment,
                ScriptDebugging = commonBuildSettings.isScriptDebugging,
                BuildPlayerOnly = commonBuildSettings.buildPlayerOnly,
                AutoConnectProfiler = commonBuildSettings.autoConnectProfiler,
                DeepProfilingSupport = commonBuildSettings.deepProfilingSupport,
                EnableObfuscator = commonBuildSettings.enableObfuscator,
            };
            GameBuildTools.ExportLinuxServerImpl(buildSettings);
        }
    }
}
