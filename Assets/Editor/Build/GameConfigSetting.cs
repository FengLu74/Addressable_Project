using System;
using System.IO;
using System.Text;
using Kit;
using TCG.Common;
using UnityEditor;
using UnityEngine;
namespace TCG.Editor {
    public static class GameConfigSetting {
        private static readonly string ConfigSourcePath = Application.dataPath + "/../CCGExport/Config/";
        private static readonly string ConfigDestAndroidPath = Application.dataPath + "/../CCGExport/Android/ccg_config/";
        private static readonly string ConfigDestIOSPath = Application.dataPath + "/../CCGExport/IOS/ccg_config/";
        private static readonly string ConfigServer = ConfigSourcePath + "Server";
        private const string AESKey = "sS@cCgGAme@HuANxInGpAi@LaTtEbAnk";
        private static string _activeTarget;

        [MenuItem("CustomTools/配置文件/开发")]
        public static void SetDevelopConfig() {
            var sourcePath = ConfigSourcePath + "开发";
            if (!CheckConfigPath(sourcePath)) {
                return;
            }
            _activeTarget = Settings.GetPlatformName().ToString().ToLower();
            CopyDone(sourcePath, false);
        }

        [MenuItem("CustomTools/配置文件/正式")]
        public static void SetOnlineConfig() {
            var sourcePath = ConfigSourcePath + "线上";
            if (!CheckConfigPath(sourcePath)) {
                return;
            }
            _activeTarget = Settings.GetPlatformName().ToString().ToLower();
            CopyDone(sourcePath);
        }

        public static void SetServerConfig(bool isEncrypt) => CopyDone(ConfigServer, isEncrypt, false);

        public static void SetDevelopConfig(string version, int build, string activeTarget = null) {
            Debug.Log("------------------- SetDevelopConfig:" + version);
            var sourcePath = ConfigSourcePath + "开发";
            if (!CheckConfigPath(sourcePath)) {
                return;
            }
            _activeTarget = (activeTarget ?? Settings.GetPlatformName().ToString()).ToLower();
            WriteVersion(sourcePath, version, build);
            CopyDone(sourcePath, false);
        }

        public static void SetOnlineConfig(string version, int build, string activeTarget = null) {
            var sourcePath = ConfigSourcePath + "线上";
            if (!CheckConfigPath(sourcePath)) {
                return;
            }
            _activeTarget = (activeTarget ?? Settings.GetPlatformName().ToString()).ToLower();
            WriteVersion(sourcePath, version, build);
            CopyDone(sourcePath);
        }

        private static void WriteVersion(string filePath, string version, int build) {
            Debug.Log("------------------- WriteVersion:" + version);
            Debug.Log("------------------- build:" + build);
            var versionPath = Path.Combine(filePath, _activeTarget);
            versionPath = Path.Combine(versionPath, "version.con");
            if (build < 0) {
                try {
                    var jsonContent = File.ReadAllText(versionPath, Encoding.UTF8);
                    build = JsonUtility.FromJson<VersionSetting>(jsonContent).build;
                } catch (Exception e) {
                    Log.LogException(e);
                    WriteVersion(filePath, version, build);
                    return;
                }
            }
            var versionSetting = new VersionSetting {version = version, build = build};
            var content = JsonUtility.ToJson(versionSetting);
            var bytes = Encoding.UTF8.GetBytes(content);

            if (File.Exists(versionPath)) {
                File.Delete(versionPath);
            }
            File.WriteAllBytes(versionPath, bytes);
        }

        private static void CopyDone(string sourcePath, bool isEncrypt = true, bool clear = true) {
            if (clear) {
                ClearAllFile(ConfigDestAndroidPath);
                ClearAllFile(ConfigDestIOSPath);
                ClearAllFile(DirectoryUtils.GetBaseFileStreamingAssetsPath("ccg_config"));
            }
            if (isEncrypt) {
                DirectoryCopyWithAction(sourcePath, ConfigDestAndroidPath, true, EncryptConfig);
                DirectoryCopyWithAction(sourcePath, ConfigDestIOSPath, true, EncryptConfig);
                DirectoryCopyWithAction(sourcePath, DirectoryUtils.GetBaseFileStreamingAssetsPath("ccg_config"), true, EncryptConfig);
            } else {
                DirectoryCopyWithAction(sourcePath, ConfigDestAndroidPath, true);
                DirectoryCopyWithAction(sourcePath, ConfigDestIOSPath, true);
                DirectoryCopyWithAction(sourcePath, DirectoryUtils.GetBaseFileStreamingAssetsPath("ccg_config"), true);
            }
            OutputFinalText();
        }

        private static void ClearAllFile(string path) {
            if (!Directory.Exists(path)) {
                return;
            }
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (var file in files) {
                if (file.EndsWith(".meta") || file.Contains("uwa")) {
                    continue;
                }
                File.Delete(file);
            }
        }

        private static bool CheckConfigPath(string sourceFolderName) {
            var sourcePath = sourceFolderName;
            if (!Directory.Exists(sourcePath)) {
                EditorUtility.DisplayDialog("Error", "配置文件路径未找到！", "确定");
                return false;
            }
            if (!Directory.Exists(ConfigDestAndroidPath)) {
                Directory.CreateDirectory(ConfigDestAndroidPath);
            }
            if (!Directory.Exists(ConfigDestIOSPath)) {
                Directory.CreateDirectory(ConfigDestIOSPath);
            }
            return true;
        }

        private static void EncryptConfig(string filename) {
            if (filename.EndsWith("dev.con")) {
                return;
            }
            var content = File.ReadAllText(filename);
            File.Delete(filename);
            AESUtils.SetKey(AESKey);
            var encrypt = AESUtils.Encrypt(content);
            File.WriteAllText(filename, encrypt);
        }

        private static void OutputFinalText() {
            Debug.Log("Android：");
            var file = Directory.GetFiles(ConfigDestAndroidPath, "*.*", SearchOption.AllDirectories);
            foreach (var f in file) {
                if (f.EndsWith(".meta")) {
                    continue;
                }
                Debug.Log(File.ReadAllText(f));
            }

            Debug.Log("IOS：");
            file = Directory.GetFiles(ConfigDestIOSPath, "*.*", SearchOption.AllDirectories);
            foreach (var f in file) {
                if (f.EndsWith(".meta")) {
                    continue;
                }
                Debug.Log(File.ReadAllText(f));
            }
        }

        private static void DirectoryCopyWithAction(string sourceDirName, string destDirName,
            bool copySubDirs, Action<string> postProcess = null) {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName)) {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files) {
                var tempPath = Path.Combine(destDirName, file.Name);
                if (File.Exists(tempPath)) {
                    File.Delete(tempPath);
                }
                file.CopyTo(tempPath, false);
                postProcess?.Invoke(tempPath);
            }

            // If copying subdirectories, copy them and their contents to new location.
            // ReSharper disable once InvertIf
            if (copySubDirs) {
                var dirs = dir.GetDirectories();
                foreach (var subdir in dirs) {
                    var tempPath = destDirName;
                    if (subdir.Name.Equals(_activeTarget)) {
                        DirectoryCopyWithAction(subdir.FullName, tempPath, true, postProcess);
                    }
                }
            }
        }
    }
}
