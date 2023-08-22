using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Beebyte.Obfuscator;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace TCG.Editor {
    public class GameBuildTools : MonoBehaviour {
        private static readonly Dictionary<string, bool> DefineSymbols = new Dictionary<string, bool> {

        };

        /// <summary>
        /// 通过字符串获取当前的API等级枚举
        /// </summary>
        /// <param name="strAndroidAPILevel"></param>
        /// <returns></returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static AndroidSdkVersions GetAndroidAPILevel(string strAndroidAPILevel) {
            var androidAPILevel = AndroidSdkVersions.AndroidApiLevel28;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (strAndroidAPILevel) {
            // case "16":
            //    androidAPILevel = AndroidSdkVersions.AndroidApiLevel16;
            //     break;
            // case "17":
            //    androidAPILevel = AndroidSdkVersions.AndroidApiLevel17;
            //     break;
            // case "18":
            //     androidAPILevel = AndroidSdkVersions.AndroidApiLevel18;
            //     break;
            // case "19":
            //     androidAPILevel = AndroidSdkVersions.AndroidApiLevel19;
            //     break;
            // case "21":
            //     androidAPILevel = AndroidSdkVersions.AndroidApiLevel21;
            //     break;
            case "22":
                androidAPILevel = AndroidSdkVersions.AndroidApiLevel22;
                break;
            case "23":
                androidAPILevel = AndroidSdkVersions.AndroidApiLevel23;
                break;
            case "24":
                androidAPILevel = AndroidSdkVersions.AndroidApiLevel24;
                break;
            case "25":
                androidAPILevel = AndroidSdkVersions.AndroidApiLevel25;
                break;
            case "26":
                androidAPILevel = AndroidSdkVersions.AndroidApiLevel26;
                break;
            case "27":
                androidAPILevel = AndroidSdkVersions.AndroidApiLevel27;
                break;
            case "28":
                androidAPILevel = AndroidSdkVersions.AndroidApiLevel28;
                break;
            }
            return androidAPILevel;
        }
        /// <summary>
        /// 获取控制台传过来的构建设置
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static BuildSettings GetBuildSettings() {
            var buildSettings = new BuildSettings();
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "-export");
            if (index != -1) {
                buildSettings.ExportPath = args[index + 1];
            }
            Debug.Log("---------------------- exportPath:" + buildSettings.ExportPath);

            index = Array.IndexOf(args, "-bundleVersionCode");
            if (index != -1) {
                buildSettings.BundleVersionCode = int.Parse(args[index + 1]);
            }
            Debug.Log("---------------------- BundleVersionCode:" +
                      buildSettings.BundleVersionCode);

            index = Array.IndexOf(args, "-productName");
            buildSettings.ProductName = PlayerSettings.productName;
            if (index != -1) {
                buildSettings.ProductName = args[index + 1];
            }
            Debug.Log("--------------------- productName:" + buildSettings.ProductName);

            index = Array.IndexOf(args, "-isRelease");
            var isRelease = "false";
            if (index != -1) {
                isRelease = args[index + 1];
                if (isRelease == "true") {
                    buildSettings.IsRelease = true;
                }
            }
            Debug.Log("------------------- IsRelease:" + isRelease);

            index = Array.IndexOf(args, "-isObfuscate");
            var isObfuscate = "false";
            if (index != -1) {
                isObfuscate = args[index + 1];
                if (isObfuscate == "true") {
                    buildSettings.EnableObfuscator = true;
                }
            }
            Debug.Log("------------------- EnableObfuscator:" + isObfuscate);

            index = Array.IndexOf(args, "-bundleIdentifier");
            if (index != -1) {
                buildSettings.BundleIdentifier = args[index + 1];
            }
            Debug.Log("-------------------- bundleIdentifier:" + buildSettings.BundleIdentifier);

            index = Array.IndexOf(args, "-bundleVersion");
            if (index != -1) {
                buildSettings.BundleVersion = args[index + 1];
            }
            buildSettings.BundleVersion = buildSettings.BundleVersion.Replace("\"", "");
            Debug.Log("------------------- bundleVersion:" + buildSettings.BundleVersion);

            index = Array.IndexOf(args, "-defineSymbols");
            if (index != -1) {
                buildSettings.DefineSymbols = args[index + 1];
            }
            Debug.Log("------------------- defineSymbols:" + buildSettings.DefineSymbols);

            var strScriptingBackend = "IL2CPP";
            index = Array.IndexOf(args, "-scriptingBackend");
            if (index != -1) {
                strScriptingBackend = args[index + 1];
                if (strScriptingBackend == "Mono2x") {
                    buildSettings.ScriptingBackend = ScriptingImplementation.Mono2x;
                }
            }
            Debug.Log("------------------- ScriptingBackend:" + strScriptingBackend);

            index = Array.IndexOf(args, "-packAssetBundle");
            var packAssetBundle = "true";
            if (index != -1) {
                packAssetBundle = args[index + 1];
                if (packAssetBundle == "false") {
                    buildSettings.PackBundleFlag = 0;
                }
            }
            Debug.Log("------------------- packAssetBundle:" + packAssetBundle);

            index = Array.IndexOf(args, "-IOSSdkVersion");
            if (index != -1) {
                buildSettings.IOSSdkVersion = args[index + 1];
            }
            Debug.Log("------------------- IOSSdkVersion:" + buildSettings.IOSSdkVersion);

            index = Array.IndexOf(args, "-androidAPILevel");
            var androidAPILevel = "28";
            if (index != -1) {
                androidAPILevel = args[index + 1];
                buildSettings.AndroidAPILevel = GetAndroidAPILevel(androidAPILevel);
            }
            Debug.Log("------------------- androidAPILevel:" + androidAPILevel);

            index = Array.IndexOf(args, "-isTest");
            var isTest = "false";
            if (index != -1) {
                isTest = args[index + 1];
                if (isTest == "true") {
                    buildSettings.IsTest = true;
                }
            }
            Debug.Log("------------------- IsTest:" + isTest);

            index = Array.IndexOf(args, "-isDevelopment");
            var isDevelopment = "false";
            if (index != -1) {
                isDevelopment = args[index + 1];
                if (isDevelopment == "true") {
                    buildSettings.IsDevelopment = true;
                }
            }
            Debug.Log("------------------- IsDevelopment:" + isDevelopment);

            index = Array.IndexOf(args, "-isScriptDebugging");
            var isScriptDebugging = "false";
            if (index != -1) {
                isScriptDebugging = args[index + 1];
                if (isScriptDebugging == "true") {
                    buildSettings.ScriptDebugging = true;
                }
            }
            Debug.Log("------------------- ScriptDebugging:" + isScriptDebugging);

            index = Array.IndexOf(args, "-buildPlayerOnly");
            var buildPlayerOnly = "false";
            if (index != -1) {
                buildPlayerOnly = args[index + 1];
                if (buildPlayerOnly == "true") {
                    buildSettings.BuildPlayerOnly = true;
                }
            }
            Debug.Log("------------------- BuildPlayerOnly:" + buildPlayerOnly);

            index = Array.IndexOf(args, "-ProfileID");
            if (index != -1) {
                buildSettings.ProfileID = args[index + 1];
            }
            Debug.Log("------------------- ProfileID:" + buildSettings.ProfileID);

            index = Array.IndexOf(args, "-ProfileType");
            if (index != -1) {
                var ret = args[index + 1];
                switch (ret) {
                case "Automatic":
                    buildSettings.ProfileType = ProvisioningProfileType.Automatic;
                    break;
                case "Development":
                    buildSettings.ProfileType = ProvisioningProfileType.Development;
                    break;
                default:
                    buildSettings.ProfileType = ProvisioningProfileType.Distribution;
                    break;
                }
            }
            Debug.Log("------------------- ProfileType:" + buildSettings.ProfileType);

            index = Array.IndexOf(args, "-TeamID");
            if (index != -1) {
                buildSettings.TeamID = args[index + 1];
            }
            Debug.Log("------------------- TeamID:" + buildSettings.TeamID);

            index = Array.IndexOf(args, "-isPackApk");
            if (index != -1) {
                var isPackApk = args[index + 1];
                if (isPackApk == "false") {
                    buildSettings.IsPackApk = false;
                }
            }
            Debug.Log("------------------- isPackApk:" +
                      (buildSettings.IsPackApk ? "true" : "false"));

            index = Array.IndexOf(args, "-isCleanAllAb");
            if (index != -1) {
                var isCleanAllAb = args[index + 1];
                if (isCleanAllAb == "true") {
                    buildSettings.IsCleanAllAb = true;
                }
            }
            Debug.Log("------------------- isCleanAllAb:" +
                      (buildSettings.IsCleanAllAb ? "true" : "false"));


            index = Array.IndexOf(args, "-autoConnectProfiler");
            if (index != -1) {
                var autoConnectProfiler = args[index + 1];
                if (autoConnectProfiler == "true") {
                    buildSettings.AutoConnectProfiler = true;
                }
            }
            Debug.Log("------------------- autoConnectProfiler:" +
                      (buildSettings.AutoConnectProfiler ? "true" : "false"));


            index = Array.IndexOf(args, "-deepProfilingSupport");
            if (index != -1) {
                var deepProfilingSupport = args[index + 1];
                if (deepProfilingSupport == "true") {
                    buildSettings.DeepProfilingSupport = true;
                }
            }
            Debug.Log("------------------- deepProfilingSupport:" +
                      (buildSettings.DeepProfilingSupport ? "true" : "false"));

            PreCopyFile(buildSettings);

            return buildSettings;
        }

        private static void PreCopyFile(BuildSettings buildSettings) {

            if (buildSettings.IsCleanAllAb) {
                AddressableBuilder.OnCleanSBP();
            }

            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            settings.OverridePlayerVersion = buildSettings.BundleVersion;
        }

        private static void SetVersionCon(BuildSettings buildSettings, string activeTarget) {

            if (buildSettings.IsRelease) {
                GameConfigSetting.SetOnlineConfig(buildSettings.BundleVersion,
                    buildSettings.BundleVersionCode + 1, activeTarget);
            } else {
                GameConfigSetting.SetDevelopConfig(buildSettings.BundleVersion,
                    buildSettings.BundleVersionCode + 1, activeTarget);
            }

        }

        // [MenuItem("Tools/CreateHotfixFilesAndroid")]
        // ReSharper disable once UnusedMember.Global
        public static void CreateHotfixFilesAndroid() {
            // ReSharper disable once UnusedVariable
            var buildSettings = GetBuildSettings();
            SetVersionCon(buildSettings, "Android");
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (!buildSettings.BuildPlayerOnly) {
                /////////step2 打包所有AB
                PackAllAssetBundle(BuildTarget.Android, buildSettings.PackBundleFlag, false);
            }
            settings.ActivePlayModeDataBuilderIndex = 2;
            AddressableBuilder.OnUpdateBuild();
            settings.ActivePlayModeDataBuilderIndex = 0;
        }

        // ReSharper disable once UnusedMember.Global
        public static void CreateHotfixFilesIos() {
            // ReSharper disable once UnusedVariable
            var buildSettings = GetBuildSettings();
            SetVersionCon(buildSettings, "Ios");
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (!buildSettings.BuildPlayerOnly) {
                /////////step2 打包所以AB
                PackAllAssetBundle(BuildTarget.iOS, buildSettings.PackBundleFlag, false);
            }
            settings.ActivePlayModeDataBuilderIndex = 2;
            AddressableBuilder.OnUpdateBuild();
            settings.ActivePlayModeDataBuilderIndex = 0;
        }

        // [MenuItem("Tools/ExportAndroid")]
        // ReSharper disable once UnusedMember.Global
        public static void ExportAndroid() {
            CleanLibraryCache();
            var buildSettings = GetBuildSettings();
            SetVersionCon(buildSettings, "Android");
            ExportAndroidImpl(buildSettings);
        }

        /// <summary>
        /// 导出安卓gradle工程
        /// </summary>
        ///  [MenuItem("Tools/ExportAndroid")]
        public static void ExportAndroidImpl(BuildSettings buildSettings) {
            SetScriptingDefineSymbolsForGroup();

            AssetDatabase.Refresh();
            if (!buildSettings.BuildPlayerOnly) {
                /////////step1 打包所有AB
                PackAllAssetBundle(BuildTarget.Android, buildSettings.PackBundleFlag,
                    buildSettings.IsPackApk);
            }
            //////////step2 XLua生成wrap代码
            GenXLuaCode();
            SetObfuscate(buildSettings.EnableObfuscator);

            var cachedSettings = new CachedSettings();
            cachedSettings.Cache(BuildTargetGroup.Android);

            var isDevelop = buildSettings.IsDevelopment;

            PlayerSettings.Android.bundleVersionCode = buildSettings.BundleVersionCode;
            PlayerSettings.productName = buildSettings.ProductName;
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,
                BuildTarget.Android);
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = !buildSettings.IsPackApk;
            EditorUserBuildSettings.development = isDevelop;
            EditorUserBuildSettings.connectProfiler = buildSettings.AutoConnectProfiler;
            EditorUserBuildSettings.allowDebugging = buildSettings.ScriptDebugging;
            EditorUserBuildSettings.waitForManagedDebugger = buildSettings.ScriptDebugging;
            EditorUserBuildSettings.buildWithDeepProfilingSupport =
                buildSettings.DeepProfilingSupport;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android,
                buildSettings.ScriptingBackend);
            PlayerSettings.Android.targetSdkVersion = buildSettings.AndroidAPILevel;
            PlayerSettings.enableInternalProfiler = isDevelop;
            var versionList = buildSettings.BundleVersion.Split('.').ToList();
            versionList.RemoveAt(versionList.Count - 1);
            PlayerSettings.bundleVersion = string.Join(".", versionList.ToArray());
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,
                buildSettings.BundleIdentifier);
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android,
                ManagedStrippingLevel.Disabled);
            //PlayerSettings.strippingLevel = StrippingLevel.Disabled;
            //PlayerSettings.SetGraphicsAPIs (BuildTarget.Android,UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            //   PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android,defineSymbols);
            PlayerSettings.MTRendering = true;
            // PlayerSettings.mobileMTRendering = true;
            PlayerSettings.statusBarHidden = true;
            // ReSharper disable once CommentTypo
            // PlayerSettings.keystorePass = "";

            var buildOptions = BuildOptions.None;
            buildOptions |= isDevelop ? BuildOptions.Development : BuildOptions.None;
            buildOptions |= isDevelop ? BuildOptions.ConnectWithProfiler : BuildOptions.None;
            buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
            if (buildSettings.ScriptDebugging) {
                buildOptions |= BuildOptions.AllowDebugging;
            }
            var pathName = buildSettings.IsPackApk
                ? $"{buildSettings.ExportPath}/{buildSettings.ExportName}_{DateTime.Now.ToString( "yyyyMMddHHmm")}.apk"
                : $"{buildSettings.ExportPath}/{buildSettings.ExportName}";
            var buildReport = BuildPipeline.BuildPlayer(GetScenes(buildSettings.IsTest), pathName, BuildTarget.Android, BuildOptions.None);
            var buildSummary = buildReport.summary;
            if (buildSummary.result == BuildResult.Succeeded) {
                Debug.Log("Build succeeded: " + buildSummary.totalSize + " bytes");
            }
            if (buildSummary.result == BuildResult.Failed) {
                Debug.Log("Build failed");
            }

            cachedSettings.Restore();

            //CSObjectWrapEditor.Generator.ClearAll();
            //CleanEmptyFolders.Cleanup(
            //    Path.GetFullPath(Path.Combine(CSObjectWrapEditor.GeneratorConfig.common_path,
            //        "..")));
        }

        // [MenuItem("Tools/ExportIOS")]
        // ReSharper disable once UnusedMember.Global
        public static void ExportIOS() {
            CleanLibraryCache();
            var buildSettings = GetBuildSettings();
            SetVersionCon(buildSettings, "IOS");
            ExportIOSImpl(buildSettings);
        }

        /// <summary>
        /// 导出XCode工程
        /// </summary>
        //[MenuItem("Tools/ExportIOS")]
        public static void ExportIOSImpl(BuildSettings buildSettings) {
            SetScriptingDefineSymbolsForGroup();

            AssetDatabase.Refresh();
            if (!buildSettings.BuildPlayerOnly) {
                /////////step1 打包所以AB
                PackAllAssetBundle(BuildTarget.iOS, buildSettings.PackBundleFlag,
                    buildSettings.IsPackApk);
            }
            //////////step2 XLua生成wrap代码
            GenXLuaCode();
            SetObfuscate(buildSettings.EnableObfuscator);

            var cachedSettings = new CachedSettings();
            cachedSettings.Cache(BuildTargetGroup.iOS);

            PlayerSettings.productName = buildSettings.ProductName;
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            EditorUserBuildSettings.development = buildSettings.IsDevelopment;
            var versionList = buildSettings.BundleVersion.Split('.').ToList();
            versionList.RemoveAt(versionList.Count - 1);
            PlayerSettings.bundleVersion = string.Join(".", versionList.ToArray());
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.targetOSVersionString = buildSettings.IOSSdkVersion;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS,
                buildSettings.BundleIdentifier);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, true);
            // PlayerSettings.iOS.iOSManualProvisioningProfileID = buildSettings.ProfileID;
            // PlayerSettings.iOS.iOSManualProvisioningProfileType = buildSettings.ProfileType;
            PlayerSettings.iOS.appleDeveloperTeamID = buildSettings.TeamID;
            //    PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS,defineSymbols);
            PlayerSettings.statusBarHidden = true;

            var buildOptions = BuildOptions.None;
            buildOptions |= buildSettings.IsDevelopment
                ? BuildOptions.Development
                : BuildOptions.None;
            buildOptions |= BuildOptions.SymlinkSources;
            var pathName = $"{buildSettings.ExportPath}/{buildSettings.ExportName}";
            BuildPipeline.BuildPlayer(GetScenes(), pathName, BuildTarget.iOS,
                buildOptions);

            cachedSettings.Restore();

            //CSObjectWrapEditor.Generator.ClearAll();
            //CleanEmptyFolders.Cleanup(
            //    Path.GetFullPath(Path.Combine(CSObjectWrapEditor.GeneratorConfig.common_path,
            //        "..")));
        }

        // ReSharper disable once UnusedMember.Global
        public static void ExportWin() {
            var buildSettings = GetBuildSettings();
            ExportWinImpl(buildSettings);
        }

        /// <summary>
        /// 导出win server版本
        /// </summary>
        ///[MenuItem("Tools/ExportWin")]
        public static void ExportWinImpl(BuildSettings buildSettings) {
            GameConfigSetting.SetServerConfig(buildSettings.IsDevelopment);

            SetScriptingDefineSymbolsForGroup();

            AssetDatabase.Refresh();
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone,
                ScriptingImplementation.Mono2x);
            if (!buildSettings.BuildPlayerOnly) {
                /////////step1 打包所以AB
                PackServerAssetBundle(BuildTarget.StandaloneWindows64, buildSettings.IsPackApk);
            }
            //////////step2 XLua生成wrap代码
            GenXLuaCode();
            SetObfuscate(buildSettings.EnableObfuscator);

            var cachedSettings = new CachedSettings();
            cachedSettings.Cache(BuildTargetGroup.Standalone);

            PlayerSettings.productName = buildSettings.ProductName;
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            EditorUserBuildSettings.development = buildSettings.IsDevelopment;
            EditorUserBuildSettings.allowDebugging = true;
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.switchEnableDebugPad = true;
            var versionList = buildSettings.BundleVersion.Split('.').ToList();
            versionList.RemoveAt(versionList.Count - 1);
            PlayerSettings.bundleVersion = string.Join(".", versionList.ToArray());
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, buildSettings.BundleIdentifier);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
            PlayerSettings.defaultScreenHeight = 1080;
            PlayerSettings.defaultScreenWidth = 1920;
            PlayerSettings.statusBarHidden = true;

            var buildOptions = BuildOptions.None;
            buildOptions |= BuildOptions.Development;
            buildOptions |= BuildOptions.AllowDebugging;
            buildOptions |= BuildOptions.CompressWithLz4HC | BuildOptions.EnableHeadlessMode;
            var pathName = $"{buildSettings.ExportPath}/{buildSettings.ExportName}.exe";
            BuildPipeline.BuildPlayer(GetScenes(true), pathName, BuildTarget.StandaloneWindows64, buildOptions);

            cachedSettings.Restore();

            //CSObjectWrapEditor.Generator.ClearAll();
            //CleanEmptyFolders.Cleanup(
            //    Path.GetFullPath(Path.Combine(CSObjectWrapEditor.GeneratorConfig.common_path,
            //        "..")));
        }

        /// <summary>
        /// 导出linux server 版本
        /// </summary>
        /// <param name="buildSettings"></param>
        public static void ExportLinuxServerImpl(BuildSettings buildSettings) {

            GameConfigSetting.SetServerConfig(buildSettings.IsDevelopment);

            SetScriptingDefineSymbolsForGroup();

            AssetDatabase.Refresh();
            if (!buildSettings.BuildPlayerOnly) {
                /////////step1 打包所以AB
                PackServerAssetBundle(BuildTarget.StandaloneLinux64, buildSettings.IsPackApk);
            }
            //////////step2 XLua生成wrap代码
            GenXLuaCode();
            SetObfuscate(buildSettings.EnableObfuscator);

            var cachedSettings = new CachedSettings();
            cachedSettings.Cache(BuildTargetGroup.Standalone);

            PlayerSettings.productName = buildSettings.ProductName;
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64);
            EditorUserBuildSettings.development = buildSettings.IsDevelopment;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, buildSettings.ScriptingBackend);
            var versionList = buildSettings.BundleVersion.Split('.').ToList();
            versionList.RemoveAt(versionList.Count - 1);
            PlayerSettings.bundleVersion = string.Join(".", versionList.ToArray());
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, buildSettings.BundleIdentifier);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneLinux64, true);
            PlayerSettings.statusBarHidden = true;


            // var buildOptions = BuildOptions.None;
            // buildOptions |= buildSettings.IsDevelopment
            //     ? BuildOptions.Development
            //     : BuildOptions.None;
            // buildOptions |= BuildOptions.SymlinkSources;
            // buildOptions |= BuildOptions.CompressWithLz4HC | BuildOptions.EnableHeadlessMode;

            // BuildPipeline.BuildPlayer(GetScenes(true), buildSettings.ExportPath, BuildTarget.StandaloneLinux64,
            //     buildOptions);

            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetScenes(true);
            buildPlayerOptions.locationPathName = $"{buildSettings.ExportPath}/Server.x86_64";
            buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
            buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Server;
            BuildPipeline.BuildPlayer(buildPlayerOptions);

            cachedSettings.Restore();

            //CSObjectWrapEditor.Generator.ClearAll();
            //CleanEmptyFolders.Cleanup(
            //    Path.GetFullPath(Path.Combine(CSObjectWrapEditor.GeneratorConfig.common_path,
            //        "..")));
        }

        /// <summary>
        /// 导出android server 版本
        /// </summary>
        /// <param name="buildSettings"></param>
        public static void ExportAndroidServerImpl(BuildSettings buildSettings) {
            SetScriptingDefineSymbolsForGroup();

            AssetDatabase.Refresh();
            if (!buildSettings.BuildPlayerOnly) {
                /////////step1 打包所有AB
                PackServerAssetBundle(BuildTarget.Android, buildSettings.IsPackApk);
            }
            //////////step2 XLua生成wrap代码
            GenXLuaCode();
            SetObfuscate(buildSettings.EnableObfuscator);

            var cachedSettings = new CachedSettings();
            cachedSettings.Cache(BuildTargetGroup.Android);

            var isDevelop = buildSettings.IsDevelopment;

            PlayerSettings.Android.bundleVersionCode = buildSettings.BundleVersionCode;
            PlayerSettings.productName = buildSettings.ProductName;
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,
                BuildTarget.Android);
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = !buildSettings.IsPackApk;
            EditorUserBuildSettings.development = isDevelop;
            EditorUserBuildSettings.connectProfiler = buildSettings.AutoConnectProfiler;
            EditorUserBuildSettings.allowDebugging = buildSettings.ScriptDebugging;
            EditorUserBuildSettings.waitForManagedDebugger = buildSettings.ScriptDebugging;
            EditorUserBuildSettings.buildWithDeepProfilingSupport =
                buildSettings.DeepProfilingSupport;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android,
                buildSettings.ScriptingBackend);
            PlayerSettings.Android.targetSdkVersion = buildSettings.AndroidAPILevel;
            PlayerSettings.enableInternalProfiler = isDevelop;
            var versionList = buildSettings.BundleVersion.Split('.').ToList();
            versionList.RemoveAt(versionList.Count - 1);
            PlayerSettings.bundleVersion = string.Join(".", versionList.ToArray());
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,
                buildSettings.BundleIdentifier);
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android,
                ManagedStrippingLevel.Disabled);
            //PlayerSettings.strippingLevel = StrippingLevel.Disabled;
            //PlayerSettings.SetGraphicsAPIs (BuildTarget.Android,UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            //   PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android,defineSymbols);
            PlayerSettings.MTRendering = true;
            // PlayerSettings.mobileMTRendering = true;
            PlayerSettings.statusBarHidden = true;
            // ReSharper disable once CommentTypo
            // PlayerSettings.keystorePass = "";

            var buildOptions = BuildOptions.None;
            buildOptions |= isDevelop ? BuildOptions.Development : BuildOptions.None;
            buildOptions |= isDevelop ? BuildOptions.ConnectWithProfiler : BuildOptions.None;
            buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
            if (buildSettings.ScriptDebugging) {
                buildOptions |= BuildOptions.AllowDebugging;
            }
            var pathName = buildSettings.IsPackApk
                ? $"{buildSettings.ExportPath}/GameServer_{DateTime.Now.ToString( "yyyyMMddHHmm")}.apk"
                : $"{buildSettings.ExportPath}/{buildSettings.ExportName}";
            var buildReport = BuildPipeline.BuildPlayer(GetScenes(true), pathName, BuildTarget.Android, BuildOptions.None);
            var buildSummary = buildReport.summary;
            if (buildSummary.result == BuildResult.Succeeded) {
                Debug.Log("Build succeeded: " + buildSummary.totalSize + " bytes");
            }
            if (buildSummary.result == BuildResult.Failed) {
                Debug.Log("Build failed");
            }

            cachedSettings.Restore();

            //CSObjectWrapEditor.Generator.ClearAll();
            //CleanEmptyFolders.Cleanup(
            //    Path.GetFullPath(Path.Combine(CSObjectWrapEditor.GeneratorConfig.common_path,
            //        "..")));
        }

        private static void SetDefineSymbolsValue(string define, bool enable) {
            if (DefineSymbols.ContainsKey(define)) {
                DefineSymbols[define] = enable;
            } else {
                DefineSymbols.Add(define, enable);
            }
        }

        private static void SetScriptingDefineSymbolsForGroup() {
            var buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defineSymbolsContent =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget);
            foreach (var defineSymbol in DefineSymbols) {
                var contains = defineSymbolsContent.Contains(defineSymbol.Key);
                if (contains == defineSymbol.Value) {
                    continue;
                }
                var symbolContent = defineSymbol.Key + ";";
                if (defineSymbol.Value) {
                    defineSymbolsContent = symbolContent + defineSymbolsContent;
                } else {
                    defineSymbolsContent =
                        defineSymbolsContent.Replace(symbolContent, string.Empty);
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTarget, defineSymbolsContent);
            AssetDatabase.Refresh();
        }

        private class CachedSettings {
            public void Cache(BuildTargetGroup buildTargetGroup) {
                _buildTargetGroup = buildTargetGroup;
                _productName = PlayerSettings.productName;
                _managedStrippingLevel = PlayerSettings.GetManagedStrippingLevel(_buildTargetGroup);
                _editorBuildSettingsScenes.Clear();
                foreach (var editorBuildSettingsScene in EditorBuildSettings.scenes) {
                    _editorBuildSettingsScenes.Add(editorBuildSettingsScene);
                }
            }
            public void Restore() {
                PlayerSettings.productName = _productName;
                PlayerSettings.SetManagedStrippingLevel(_buildTargetGroup, _managedStrippingLevel);
                EditorBuildSettings.scenes = _editorBuildSettingsScenes.ToArray();
            }

            private BuildTargetGroup _buildTargetGroup;
            private string _productName;
            private ManagedStrippingLevel _managedStrippingLevel;
            private readonly List<EditorBuildSettingsScene> _editorBuildSettingsScenes
                = new List<EditorBuildSettingsScene>();
        }

        // /// <summary>
        // /// 重新生成xLua代码
        // /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public static void GenXLuaCode() {
            //CSObjectWrapEditor.Generator.ClearAll();
            //CSObjectWrapEditor.Generator.GenAll();
        }

        public static void GenProto() {
            //GenProtoFile.GenProtoCSFile();
            //GenProtoFile.GenProtoLuaFile();
        }

        /// <summary>
        /// 是否开启混淆
        /// </summary>
        /// <param name="enable"></param>
        public static void SetObfuscate(bool enable) {
            //var options = OptionsManager.LoadOptions();
            //if (options != null) {
            //    options.enabled = enable;
            //    Debug.Log($"设置混淆状态:{options.enabled}");
            //}
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 生成所以AB
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable UnusedParameter.Global
        public static void PackAllAssetBundle(BuildTarget target,
                                              // ReSharper restore UnusedParameter.Global
                                              int packBundleFlag = (int)EnumAssetBundleType.All,
                                              bool isPackApk = true) {
            if (packBundleFlag <= 0) {
                return;
            }
            //ClearTMPFontAssetData.DoClear();

            AddressableBuilder.PackAllAddressableAsset(isPackApk);
        }

        public static void PackServerAssetBundle(BuildTarget target, bool isPackApk = true) {
            //ClearTMPFontAssetData.DoClear();

            AddressableBuilder.PackServerAddressableAsset(isPackApk);
        }

        /// <summary>
        /// 打Ab测试
        /// </summary>
        //  [MenuItem("Tools/PackAllAssetBundleTest")]
        // ReSharper disable once UnusedMember.Global
        public static void PackAllAssetBundleTest() => PackAllAssetBundle(BuildTarget.Android);

        // ReSharper disable once UnusedMember.Global
        public static void PackAssetBundle() {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "-packFlag");
            var packFlag = (int)EnumAssetBundleType.All;
            if (index != -1) {
                packFlag = int.Parse(args[index + 1]);
            }
            Debug.Log("---------packFlag: " + packFlag);

            index = Array.IndexOf(args, "-isPackApk");
            var isPackApk = true;
            if (index != -1) {
                var tmp = args[index + 1];
                if (tmp == "false") {
                    isPackApk = false;
                }
            }
            Debug.Log("---------isPackApk: " + isPackApk);
            PackAllAssetBundle(BuildTarget.Android, packFlag, isPackApk);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        //   [MenuItem("Tools/DeleteFolderTest")]
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once StringLiteralTypo
        public static void DeleteFolderTest() => DeleteFolder(
            @"C:\Users\zhengxichao\.jenkins\workspace\AutoBuild_Android\Client\Assets\StreamingAssets\assetbundle\prefabs");

        /// <summary>
        ///  清空指定的文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isIncHide">是否包含隐藏目录</param>
        // ReSharper disable once MemberCanBePrivate.Global
        public static void DeleteFolder(string path, bool isIncHide = true) {
            if (!Directory.Exists(path)) {
                return;
            }
            foreach (var d in Directory.GetFileSystemEntries(path)) {
                if (File.Exists(d)) {
                    var fi = new FileInfo(d);
                    if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                        fi.Attributes = FileAttributes.Normal;
                    }
                    File.Delete(d); //直接删除其中的文件
                } else {
                    if (!isIncHide) {
                        var dir = new DirectoryInfo(d);
                        if ((dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) {
                            continue;
                        }
                    }
                    DeleteFolder(d); ////递归删除子文件夹
                }
                // ReSharper disable once InvertIf
                if (Directory.Exists(d)) {
                    var subDir = new DirectoryInfo(d);
                    if ((subDir.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                        subDir.Attributes = FileAttributes.Normal;
                    }
                    Directory.Delete(d);
                }
            }
        }
        // ReSharper disable once UnusedMember.Global
        public static void DeleteFile(string path) {
            // ReSharper disable once InvertIf
            if (File.Exists(path)) {
                var fi = new FileInfo(path);
                if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                    fi.Attributes = FileAttributes.Normal;
                }
                File.Delete(path); //直接删除其中的文件
            }
        }

        /// <summary>
        /// 获取需要打到包里的场景
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static string[] GetScenes(bool isServer = false) {
            var editorScenes = new List<string>();
            if (isServer) {
                editorScenes.Add("Assets/Scenes/GameServer.unity");
            } else {
                foreach (var scene in EditorBuildSettings.scenes) {
                    if (scene.enabled) {
                        editorScenes.Add(scene.path);
                    }
                }
            }

            return editorScenes.ToArray();
        }



        private static void CleanLibraryCache() {
            var atlasCache = Application.dataPath + "/../Library/AtlasCache";
            DeleteFolder(atlasCache);
        }
    }
}
