using System;
using Sirenix.OdinInspector;
using UnityEditor;
namespace TCG.Editor {
    [Serializable]
    public class IOSBuildSettings {
        [HideLabel, BoxGroup] public CommonBuildSettings commonBuildSettings;
        [BoxGroup("IOS", false)]
        [LabelText("脚本后端")]
        [EnumPaging]
        public ScriptingImplementation scriptingImplementation = ScriptingImplementation.IL2CPP;
        [BoxGroup("IOS", false)]
        [LabelText("Sdk版本")]
        public string targetOSVersionString = "10.0";
        [BoxGroup("IOS", false)]
        [LabelText("Team Id")]
        public string appleDeveloperTeamID = "";
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
                IOSSdkVersion = targetOSVersionString,
                TeamID = appleDeveloperTeamID,
                IsDevelopment = commonBuildSettings.isDevelopment,
                ScriptDebugging = commonBuildSettings.isScriptDebugging,
                BuildPlayerOnly = commonBuildSettings.buildPlayerOnly,
                AutoConnectProfiler = commonBuildSettings.autoConnectProfiler,
                DeepProfilingSupport = commonBuildSettings.deepProfilingSupport,
                EnableObfuscator = commonBuildSettings.enableObfuscator,
            };
            GameBuildTools.ExportIOSImpl(buildSettings);
        }
    }
}
