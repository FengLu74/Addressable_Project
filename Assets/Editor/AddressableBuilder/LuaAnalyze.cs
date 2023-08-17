using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace TCG.Editor {
    [AddressableGroup(EBuildType.Lua)]
    public class LuaAnalyze : GroupAnalyzeBase<LuaAnalyze> {
        private const string LuaPath = "/BundleText/Lua";
        private const string LuaGameConfig = "/Design/Lua";
        private const string GroupName = "Lua";

        public override void AnalyzeGroup() {
            try {
                GenerateLua();
            } catch (Exception e) {
                EditorUtility.ClearProgressBar();
                Debug.LogError(e);
                return;
            }
            AddGroupDir("BundleText/LuaBundle");
            CreateGroup();
        }

        protected override string GetResourceName(string resName) {
            var newResName = resName.Replace("\\", "/").
                Replace("Assets/BundleText/LuaBundle/", "").
                Replace(".bytes", "");
            return newResName;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static void GenerateLua() {
            var luaBundle = Application.dataPath + "/BundleText/LuaBundle";
            var luaSourcePath = Application.dataPath + LuaPath;
            var luaConfigPath = Application.dataPath + LuaGameConfig;


            var listFiles = new List<string>();
            if (Directory.Exists(luaBundle)) {
                CommonBuilderUtils.CollectFiles(luaBundle, ref listFiles,
                    new List<string> {".bytes"});

                foreach (var listFile in listFiles) {
                    File.Delete(listFile);
                }

                listFiles.Clear();
            }

            ConvertByteCode();
            var fileTypes = new List<string> {".bytes", ".pb", ".xml"};
            // var fileTypes = new List<string> {".bytes", ".pb", ".lua", ".Lua", ".xml"};

            CommonBuilderUtils.CollectFiles(luaSourcePath, ref listFiles, fileTypes);
            CommonBuilderUtils.CollectFiles(luaConfigPath, ref listFiles, fileTypes);

            foreach (var file in listFiles)
            {
                if (file.Contains(".idea")) {
                    continue;
                }
                if (file.Contains("_Demo")) {
                    continue;
                }

                if (!File.Exists(file))
                {
                    Debug.LogError ("there no lua asset exit:" + file);
                    continue;
                }

                var luaPath = file.Contains(luaSourcePath) ? luaSourcePath : luaConfigPath;
                var newName = file.Replace(luaPath, luaBundle);
                if (!newName.EndsWith(".bytes"))
                {
                    newName = newName.Substring(0, newName.LastIndexOf('.'));
                    newName += ".bytes";
                }

                var newPath = newName.Replace(Path.GetFileName(newName), "");
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                File.Copy(file, newName);
            }
            listFiles.Clear();

            AssetDatabase.Refresh (ImportAssetOptions.ForceSynchronousImport);
        }

        //[MenuItem("Tools/混淆工具/Convert Lua ByteCode", false, 1)]
        //public static void GenerateByteCode() => GenerateLua();

        private static void ConvertByteCode() {
            AssetDatabase.DisallowAutoRefresh();
            var luaBundle = Application.dataPath + "/BundleText/LuaBundle";
            var listFiles = new List<string>();
            if (Directory.Exists(luaBundle)) {
                CommonBuilderUtils.CollectFiles(luaBundle, ref listFiles,
                    new List<string> {".bytes"});

                foreach (var listFile in listFiles) {
                    File.Delete(listFile);
                }

                listFiles.Clear();
            }

            var luacexe = "luac53.exe";
#if UNITY_ANDROID
            luacexe = "luac53.exe";
#elif UNITY_EDITOR_WIN
            luacexe = "luac53_Server.exe";
#endif

            var toolPyPath = Application.dataPath + "/../../PyTools/tools.py";

            var cmdInStr = "python " + toolPyPath + " gen_lua_byte --luac-exe " + luacexe;
            // ReSharper disable once UseObjectOrCollectionInitializer
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C " + cmdInStr;
#if UNITY_EDITOR_OSX
            cmdInStr = " " + toolPyPath + " gen_lua_byte";
            process.StartInfo.FileName = "/usr/bin/python3";
            process.StartInfo.Arguments = cmdInStr;
#endif
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            using (var reader = process.StandardError) {
                var result = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(result)) {
                    Debug.LogWarning(result);
                }
            }
            using (var reader = process.StandardOutput) {
                var result = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(result)) {
                    Debug.Log(result);
                }
            }

            process.WaitForExit();
            process.Dispose();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.AllowAutoRefresh();
        }

        protected override string GetGroupName(string groupName) => GroupName;

        public override void DeleteGroup() => DeleteGroupByPrefix(GroupName);
    }
}
