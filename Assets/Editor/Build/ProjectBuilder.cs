using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
namespace TCG.Editor {
    public class ProjectBuilder : OdinEditorWindow {

        [MenuItem("CustomTools/打包相关/工程打包")]
        private static void OpenWindow() {
            var projectBuilder = GetWindow<ProjectBuilder>();
            projectBuilder.Show();

        }

        protected override void OnEndDrawEditors() {
            base.OnEndDrawEditors();
            androidBuildSettings.commonBuildSettings.exportPath = "Build/Android";
            iOSBuildSettings.commonBuildSettings.exportPath = "Build/iOS";
            winBuildSettings.commonBuildSettings.exportPath = "Build/Win";
            linuxBuildSettings.commonBuildSettings.exportPath = "Build/Linux";
            androidServerBuildSettings.commonBuildSettings.exportPath = "Build/AndroidServer";
            winBuildSettings.commonBuildSettings.enableObfuscator = false;
            linuxBuildSettings.commonBuildSettings.enableObfuscator = false;
            androidServerBuildSettings.commonBuildSettings.enableObfuscator = false;
        }

        // ReSharper disable NotAccessedField.Global
        [TabGroup("Android"), HideLabel] public AndroidBuildSettings androidBuildSettings;
        [TabGroup("IOS"), HideLabel] public IOSBuildSettings iOSBuildSettings;
        [TabGroup("Win Server"), HideLabel] public WinBuildSettings winBuildSettings;
        [TabGroup("Linux Server"), HideLabel] public LinuxBuildSettings linuxBuildSettings;
        [TabGroup("Android Server"), HideLabel] public AndroidServerBuildSettings androidServerBuildSettings;
        // ReSharper restore NotAccessedField.Global

        public static void DirectoryCopy(string sourceDirName, string destDirName,
            bool copySubDirs) {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName)) {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files) {
                var tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            // ReSharper disable once InvertIf
            if (copySubDirs) {
                foreach (var subdir in dirs) {
                    var tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, true);
                }
            }
        }
        public static void ProcessCmd(string cmd, string cwd = null,
            Dictionary<string, string> env = null) {
            using (var process = new System.Diagnostics.Process()) {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/C " + cmd;
#if UNITY_EDITOR_OSX
                var cmdArray = cmd.Split(' ');
                process.StartInfo.FileName = cmdArray[0];
                if (cwd != null) {
                    process.StartInfo.FileName = Path.Combine(cwd, process.StartInfo.FileName);
                }
                if (cmdArray.Length > 1) {
                    process.StartInfo.Arguments =
                        string.Join(" ", cmdArray, 1, cmdArray.Length - 1);
                }
#endif
                if (cwd != null) {
                    process.StartInfo.WorkingDirectory = cwd;
                }
                if (env != null) {
                    foreach (var kv in env) {
                        if (process.StartInfo.Environment.ContainsKey(kv.Key)) {
                            process.StartInfo.Environment[kv.Key] = kv.Value;
                        } else {
                            process.StartInfo.Environment.Add(kv.Key, kv.Value);
                        }
                    }
                }
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                // using (var reader = process.StandardError) {
                //     var result = reader.ReadToEnd();
                //     if (!string.IsNullOrEmpty(result)) {
                //         Debug.LogWarning(result);
                //     }
                // }
                // using (var reader = process.StandardOutput) {
                //     var result = reader.ReadToEnd();
                //     if (!string.IsNullOrEmpty(result)) {
                //         Debug.Log(result);
                //     }
                // }
                while (!process.StandardError.EndOfStream) {
                    Debug.LogError(process.StandardError.ReadLine());
                }

                while (!process.StandardOutput.EndOfStream) {
                    Debug.Log(process.StandardOutput.ReadLine());
                }

                process.WaitForExit();
            }
        }
    }
}
