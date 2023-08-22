using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
namespace TCG.Editor {
    [Serializable]
    public class AndroidBuildSettings {
        [HideLabel, BoxGroup] public CommonBuildSettings commonBuildSettings;
        [BoxGroup("Android", false)]
        [LabelText("脚本后端")]
        [EnumPaging]
        public ScriptingImplementation scriptingImplementation = ScriptingImplementation.IL2CPP;
        [BoxGroup("Android", false)]
        [LabelText("Sdk版本")]
        [EnumPaging]
        public AndroidSdkVersions androidSdkVersions = AndroidSdkVersions.AndroidApiLevel28;
        [BoxGroup("Android", false)]
        [LabelText("JAVA_HOME")]
        [FolderPath(AbsolutePath = true, RequireExistingPath = true)]
        public string javaHome = "C:\\Program Files\\Java\\jdk1.8.0_202";
        [LabelText("是否是打包Apk")] public bool buildApk;

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
                AndroidAPILevel = androidSdkVersions,
                IsDevelopment = commonBuildSettings.isDevelopment,
                ScriptDebugging = commonBuildSettings.isScriptDebugging,
                BuildPlayerOnly = commonBuildSettings.buildPlayerOnly,
                AutoConnectProfiler = commonBuildSettings.autoConnectProfiler,
                DeepProfilingSupport = commonBuildSettings.deepProfilingSupport,
                IsPackApk = buildApk,
                EnableObfuscator = commonBuildSettings.enableObfuscator,
            };
            GameBuildTools.ExportAndroidImpl(buildSettings);

            if (buildApk) {
                var psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe") {
                    Arguments = "/e,/select," + buildSettings.ExportPath
                };
                System.Diagnostics.Process.Start(psi);
                return;
            }

            var exportFullPath = Path.GetFullPath(commonBuildSettings.exportPath);
            var buildToolsFullPath = Path.GetFullPath("../BuildTools");
            var bat = Application.platform == RuntimePlatform.OSXEditor
                ? "./gradlew"
                : "gradlew.bat";
            var gradlewPath = Path.Combine(exportFullPath, bat);
            if (!File.Exists(gradlewPath)) {
                File.Copy(Path.Combine(buildToolsFullPath, bat), gradlewPath);
            }
            var gradlePath = Path.Combine(exportFullPath, "gradle");
            if (!Directory.Exists(gradlePath)) {
                ProjectBuilder.DirectoryCopy(Path.Combine(buildToolsFullPath, "gradle"),
                    gradlePath, true);
            }
            var events = new Dictionary<string, string> {{"JAVA_HOME", javaHome}};
            ProjectBuilder.ProcessCmd(
                $"{bat} clean assembleDebug --stacktrace --info -b build.gradle",
                exportFullPath, events);
        }
    }
}
