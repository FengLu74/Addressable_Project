using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Kit;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Pipeline.Utilities;
using Debug = UnityEngine.Debug;
namespace TCG.Editor {

    public enum EBuildType {
        UI,
        Texture,
        Scene,
        Lua,
        DesignData,
        Server,
        Spine,
        Audio,
    }

    public class AddressableBuilder : OdinEditorWindow {

        [Serializable]
        public class BuildToggle {
            [ToggleGroup("enabled", "$BuildType")]
            public bool enabled;
            public EBuildType BuildType { get; private set; }

            public BuildToggle(EBuildType analyze) {
                BuildType = analyze;
                enabled = false;
            }
        }

        [LabelText("分组列表")]
        public List<BuildToggle> analyzeList = new List<BuildToggle>();

        [ButtonGroup]
        [Button(ButtonSizes.Large, Name = "全选")]
        public void SelectAll() {
            _instance._select.Clear();
            foreach (var toggle in _instance.analyzeList) {
                toggle.enabled = true;
                _instance._select.Add(toggle.BuildType, toggle.enabled);
            }
        }

        [ButtonGroup]
        [Button(ButtonSizes.Large, Name = "取消全选")]
        public void UnSelectAll() {
            foreach (var toggle in _instance.analyzeList) {
                toggle.enabled = false;
            }
            _instance._select.Clear();
        }

        [Button(ButtonSizes.Large, Name = "分组选中资源")]
        private void BuildGroup() {
            RefreshSelectToggle();
            PackAddressableAsset(_analyzeActions, _select);
        }

        [Button(ButtonSizes.Large, Name = "删除选中分组")]
        private void DeleteGroup() {
            RefreshSelectToggle();
            DeleteGroupInfo(_instance._select, _instance._deleteActions);
        }

        [Button(ButtonSizes.Large, Name = "删除所有打包资源")]
        private void DeleteAllPacketRes() => OnCleanAll();
        //
        // [Button(ButtonSizes.Large, Name = "删除Addressable所有配置文件")]
        // private void DeleteAllAddressableConfig() => OnCleanAddressable(null);
        //
        // [Button(ButtonSizes.Large, Name = "删除所有AB包")]
        // private void DeleteAllAssetBundle() => OnCleanSBP();
        //
        // [Button(ButtonSizes.Large, Name = "一键热更")]
        // private void HotfixBuildRes() => OnUpdateBuild();
        //
        // [Button(ButtonSizes.Large, Name = "打包资源")]
        // private void PackageRes() => Package();


        private static AddressableBuilder _instance;
        private readonly Dictionary<EBuildType, Action> _analyzeActions = new Dictionary<EBuildType, Action>();
        private readonly Dictionary<EBuildType, Action> _deleteActions = new Dictionary<EBuildType, Action>();
        private readonly Dictionary<EBuildType, bool> _select = new Dictionary<EBuildType, bool>();

        protected override void OnEnable() {
            base.OnEnable();
            _instance = this;
        }

        private void OnDisable() => _instance = null;

        [MenuItem("CustomTools/资源管理/资源分组工具")]
        private static void OpenWindow() {
            GetWindow<AddressableBuilder>().Show();
            _instance.analyzeList.Clear();
            _instance.analyzeList.Add(new BuildToggle(EBuildType.UI));
            _instance.analyzeList.Add(new BuildToggle(EBuildType.Lua));
            _instance.analyzeList.Add(new BuildToggle(EBuildType.Texture));
            _instance.analyzeList.Add(new BuildToggle(EBuildType.DesignData));
            _instance.analyzeList.Add(new BuildToggle(EBuildType.Server));
            _instance.analyzeList.Add(new BuildToggle(EBuildType.Spine));
            _instance.analyzeList.Add(new BuildToggle(EBuildType.Audio));
            InitAnalyze(_instance._analyzeActions, _instance._deleteActions, _instance._select);
        }

        public static void PackAllAddressableAsset(bool isPackApk) {
            OnCleanAll();
            var analyze = new Dictionary<EBuildType, Action>();
            var delete = new Dictionary<EBuildType, Action>();
            var select = new Dictionary<EBuildType, bool>();
            InitAnalyze(analyze, delete, select);
            foreach (var kvp in analyze) {
                select[kvp.Key] = true;
            }
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            settings.ActivePlayModeDataBuilderIndex = 2;
            PackAddressableAsset(analyze, select);
            if (isPackApk) {
                Package();
            }
            settings.ActivePlayModeDataBuilderIndex = 0;
        }

        public static void PackServerAddressableAsset(bool isPackApk) {
            OnCleanAll();
            var analyze = new Dictionary<EBuildType, Action>();
            var delete = new Dictionary<EBuildType, Action>();
            var select = new Dictionary<EBuildType, bool>();
            InitAnalyze(analyze, delete, select);
            foreach (var kvp in analyze) {
                select[kvp.Key] = true;
            }
            DeleteGroupInfo(select, delete);
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            settings.ActivePlayModeDataBuilderIndex = 2;

            foreach (var kvp in analyze) {
                if (kvp.Key != EBuildType.Lua && kvp.Key != EBuildType.Server && kvp.Key != EBuildType.DesignData) {
                    select[kvp.Key] = false;
                    continue;
                }
                select[kvp.Key] = true;
            }
            PackAddressableAsset(analyze, select);
            if (isPackApk) {
                Package();
            }
            settings.ActivePlayModeDataBuilderIndex = 0;
        }


        private static void InitAnalyze(IDictionary<EBuildType, Action> analyze, IDictionary<EBuildType, Action> delete, IDictionary<EBuildType, bool> select) {
            analyze.Clear();
            delete.Clear();
            const BindingFlags bindFlag = BindingFlags.Static | BindingFlags.Public |
                                          BindingFlags.FlattenHierarchy | BindingFlags.GetProperty;
            var analyzeTypeList = ReflectionUtils.GetAllTypeOfAttribute<AddressableGroupAttribute>(Assembly.GetExecutingAssembly());
            foreach (var type in analyzeTypeList) {
                var groupAttrib = type.GetCustomAttribute<AddressableGroupAttribute>(false);
                var property = type.GetProperty("Instance", bindFlag);
                if (property == null) {
                    continue;
                }
                var instance = property.GetValue(null);
                if (instance is IGroupAnalyze groupAnalyze) {
                    analyze.Add(groupAttrib.AnalyzeType, groupAnalyze.AnalyzeGroup);
                }
                if (instance is IDeleteGroup groupDelete) {
                    delete.Add(groupAttrib.AnalyzeType, groupDelete.DeleteGroup);
                }
            }
            select.Clear();
            foreach (var kvp in analyze) {
                select.Add(kvp.Key, false);
            }
        }

        private static void RefreshSelectToggle() {
            _instance._select.Clear();
            foreach (var toggle in _instance.analyzeList) {
                _instance._select.Add(toggle.BuildType, toggle.enabled);
            }
        }

        private static void PackAddressableAsset(Dictionary<EBuildType, Action> analyze,
            IReadOnlyDictionary<EBuildType, bool> select) {
            AddressableAssetSettingsDefaultObject.Settings.RemoveMissingGroup();
            AddressableAssetSettingsDefaultObject.Settings.RemoveMissingEntry();
            var sw = new Stopwatch();
            sw.Start();
            foreach (var build in analyze) {
                if (!select[build.Key]) {
                    continue;
                }

                try {
                    build.Value.Invoke();
                } catch (Exception e) {
                    Console.WriteLine(e);
                    throw;
                }
            }

            CheckGroupValid();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            BuildLocalCatalog();

            var costTime = sw.ElapsedMilliseconds / 1000f + "秒";
            Debug.Log(
                $"<color=magenta> 资源分组完成 Success !</color> <color=green>耗时:{costTime}</color>");
            sw.Reset();
        }

        private static void Package() {
            var sw = new Stopwatch();
            sw.Start();
            AddressableAssetSettings.BuildPlayerContent();
            var costTime = sw.ElapsedMilliseconds / 1000f + "秒";
            Debug.Log(
                $"<color=magenta> 资源打包完成 Success !  </color> <color=green>耗时:{costTime}</color>");
            sw.Reset();
        }

        private static void DeleteGroupInfo(IDictionary<EBuildType, bool> select, Dictionary<EBuildType, Action> deleteActions) {
            foreach (var build in deleteActions) {
                if (!select[build.Key]) {
                    continue;
                }

                try {
                    build.Value.Invoke();
                } catch (Exception e) {
                    Debug.LogError(e);
                }
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        public static void CheckGroupValid() {
            var setting = AddressableAssetSettingsDefaultObject.Settings;
            var remove = new List<AddressableAssetEntry>();
            var groups = setting.groups;
            foreach (var gp in groups) {
                if (gp.Default) {
                    continue;
                }

                if (gp.ReadOnly) {
                    continue;
                }

                try {
                    var schema = gp.GetSchema<BundledAssetGroupSchema>();
                    var loadValue = schema.LoadPath.GetValue(setting);
                    var buildValue = schema.BuildPath.GetValue(setting);
                    if (schema != null) {
                        if (!buildValue.Equals(AddressableAssetSettings.kLocalBuildPath)
                           ) {
                            var pvr = new ProfileValueReference();
                            pvr.SetVariableByName(setting,
                                AddressableAssetSettings.kLocalBuildPath);
                            schema.BuildPath = pvr;
                        }

                        if (!loadValue.Equals(AddressableAssetSettings.kLocalLoadPath)) {
                            var pvr = new ProfileValueReference();
                            pvr.SetVariableByName(setting,
                                AddressableAssetSettings.kLocalLoadPath);
                            schema.LoadPath = pvr;
                        }
                    }
                } catch (Exception e) {
                    Debug.LogError(e);
                    throw;
                }

                remove.Clear();
                foreach (var entry in gp.entries) {
                    if (string.IsNullOrEmpty(entry.AssetPath)) {
                        remove.Add(entry);
                    }
                }

                foreach (var en in remove) {
                    gp.RemoveAssetEntry(en);
                }
            }

            ClearGroup();
        }

        private static void ClearGroup() {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) {
                return;
            }

            for (var i = settings.groups.Count - 1; i >= 0; i--) {
                if (settings.groups[i].Default) {
                    continue;
                }

                if (settings.groups[i].ReadOnly) {
                    continue;
                }

                if (settings.groups[i].entries.Count == 0) {
                    settings.RemoveGroup(settings.groups[i]);
                }
            }
        }

        public static void BuildLocalCatalog() {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings.ActivePlayModeDataBuilderIndex != 0) {
                return;
            }

            // ReSharper disable StringLiteralTypo
            const string catalog =
                "ArtExport/com.unity.addressables/catalog_BuildScriptFastMode.json";
            const string setting =
                "ArtExport/com.unity.addressables/settings_BuildScriptFastMode.json";
            // ReSharper restore StringLiteralTypo
            if (File.Exists(catalog)) {
                File.Delete(catalog);
            }
            if (File.Exists(setting)) {
                File.Delete(setting);
            }

            settings.ActivePlayModeDataBuilder.BuildData<AddressablesPlayModeBuildResult>(
                new AddressablesDataBuilderInput(settings));
        }

        /// <summary>
        /// 删除所有打包资源
        /// </summary>
        private static void OnCleanAll() {
            OnCleanAddressable(null);
            OnCleanSBP();
        }

        /// <summary>
        /// 删除Addressable所有配置文件
        /// </summary>
        /// <param name="builder"></param>
        private static void OnCleanAddressable(object builder) =>
            AddressableAssetSettings.CleanPlayerContent(builder as IDataBuilder);

        /// <summary>
        /// 删除所有AB包
        /// </summary>
        public static void OnCleanSBP() {
            Debug.Log("-------->OnCleanSBP start");
            BuildCache.PurgeCache(true);
            Debug.Log("-------->OnCleanSBP finish");
        }

        public static void OnUpdateBuild() {
            var path = ContentUpdateScript.GetContentStateDataBasePath();
            if (!string.IsNullOrEmpty(path)) {
                ContentUpdateScript.BuildContentUpdate(
                    AddressableAssetSettingsDefaultObject.Settings, path);
            }
        }
    }
}
