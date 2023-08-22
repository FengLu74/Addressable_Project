using System;
using Sirenix.OdinInspector;
namespace TCG.Editor {
    [Serializable]
    public class WinBuildSettings {
        [HideLabel, BoxGroup] public CommonBuildSettings commonBuildSettings;
        [Button(ButtonSizes.Large)]
        private void Build() {
            commonBuildSettings.ProcessClearBuild();
            var buildSettings = new BuildSettings {
                BundleVersionCode = commonBuildSettings.bundleVersionCode,
                ExportPath = commonBuildSettings.exportPath,
                ProductName = commonBuildSettings.productName,
                BundleIdentifier = commonBuildSettings.applicationIdentifier,
                BundleVersion = commonBuildSettings.bundleVersion,
                IsDevelopment = commonBuildSettings.isDevelopment,
                ScriptDebugging = commonBuildSettings.isScriptDebugging,
                BuildPlayerOnly = commonBuildSettings.buildPlayerOnly,
                AutoConnectProfiler = commonBuildSettings.autoConnectProfiler,
                DeepProfilingSupport = commonBuildSettings.deepProfilingSupport,
                EnableObfuscator = commonBuildSettings.enableObfuscator,
            };
            GameBuildTools.ExportWinImpl(buildSettings);
        }
    }
}
