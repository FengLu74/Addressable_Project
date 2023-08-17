using System;
using System.Collections.Generic;
using System.IO;
using Kit;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Utilities;
using UnityEngine;
namespace TCG.Editor {
    internal interface IGroupAnalyze {
        void AnalyzeGroup();
    }

    internal interface IDeleteGroup {
        void DeleteGroup();
    }

    public abstract class GroupAnalyzeBase<T> : Singleton<T>, IGroupAnalyze, IDeleteGroup
        where T : class, new() {
        private readonly HashSet<string> _groupDir = new HashSet<string>(); // 按文件夹做资源分组(包含子文件夹)
        private readonly HashSet<string> _groupEach = new HashSet<string>(); // 按文件夹做资源分组(不包含子文件夹,每个子文件夹单独分组)
        private readonly HashSet<string> _groupFile = new HashSet<string>(); // 按单个文件资源分组
        private const string GroupPrefix = "_Gp_";
        protected const string DependPrefix = "Depend_";
        protected readonly List<string> GroupFile = new List<string>();

        public abstract void AnalyzeGroup();

        public virtual void DeleteGroup() {
            AddressableAssetSettingsDefaultObject.Settings.RemoveMissingGroup();
            AddressableAssetSettingsDefaultObject.Settings.RemoveMissingEntry();
        }

        protected virtual string GetLabel() => "";


        protected virtual string GetGroupName(string groupName) =>
            groupName.Replace("/", "_").Replace("\\", "_");

        protected virtual string GetResourceName(string resName) =>
            Path.GetFileNameWithoutExtension(resName);

        private static bool IsPathValidForEntry(string path)
        {
            if (string.IsNullOrEmpty(path)) {
                return false;
            }
            path = path.ToLower();
            if (path == CommonStrings.UnityEditorResourcePath ||
                path == CommonStrings.UnityDefaultResourcePath ||
                path == CommonStrings.UnityBuiltInExtraPath) {
                return false;
            }
            var ext = Path.GetExtension(path);
            return ext != ".cs" && ext != ".dll" && ext != ".meta";
        }

        protected virtual bool CollectGroupFile(string dir, bool withChildDir = false)
        {
            if (string.IsNullOrEmpty(dir)) {
                return false;
            }
            var targetDir = Application.dataPath + "/" + dir;
            if (!Directory.Exists(targetDir)) {
                return false;
            }

            if (AddressableAssetSettingsDefaultObject.Settings.GroupTemplateObjects.Count == 0) {
                return false;
            }

            GroupFile.Clear();
            var files = Directory.GetFiles (targetDir, "*.*", withChildDir? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (var file in files) {
                if (file.Contains(".meta") || file.Contains(".svn") || file.Contains(".git")) {
                    continue;
                }
                var newFile = file.Replace(Application.dataPath, "Assets").Replace("\\", "/");
                GroupFile.Add(newFile);
            }

            return GroupFile.Count != 0;
        }

        private static long GetFileSize(string file) {
            if (!File.Exists(file)) {
                return 0;
            }

            var info = new FileInfo(file);
            return info.Length;
        }

        protected void AddGroupDir(string dir) {
            if (string.IsNullOrEmpty(dir)) {
                return;
            }

            _groupDir.Add(dir);
        }

        protected void AddGroupEachDir(string dir)
        {
            if (string.IsNullOrEmpty(dir)) {
                return;
            }

            _groupEach.Add(dir);
        }

        protected void AddGroupFile(string dir) {
            if (string.IsNullOrEmpty(dir)) {
                return;
            }

            _groupFile.Add(dir);
        }

        private void CreateAddressableGroupFiles(string groupName, List<string> files) {
            var group = CreateNewGroup(groupName);
            if (group == null) {
                return;
            }
            RemoveEntry(group);
            foreach (var file in files) {
                var guid = AssetDatabase.AssetPathToGUID(file);
                var entry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid);
                if (entry != null) {
                    if (entry.parentGroup != null && entry.parentGroup != group) {
                        AddressableAssetSettingsDefaultObject.Settings.MoveEntry(entry, group);
                    }
                    continue;
                }
                if (string.IsNullOrEmpty(guid) || !IsPathValidForEntry(file)) {
                    continue;
                }
                entry = group.GetAssetEntry(guid);
                if (entry != null) {
                    continue;
                }
                var add = false;
                foreach (var en in group.entries) {
                    if (!en.AssetPath.Equals(file, StringComparison.OrdinalIgnoreCase)) {
                        continue;
                    }
                    add = true;
                    break;
                }
                if (add) {
                    continue;
                }
                entry = new AddressableAssetEntry(guid, GetResourceName(file), group);
                entry.SetLabel(GetLabel(), true);
                group.AddAssetEntryEx(entry);
            }
        }

        private AddressableAssetGroup CreateNewGroup(string groupName) {
            var newGroupName = GetGroupName(groupName);
            var targetGroup = AddressableAssetSettingsDefaultObject.Settings.groups.Find(o => o.Name.Equals(newGroupName));
            if (targetGroup != null) {
                return targetGroup;
            }
            var groupObj = AddressableAssetSettingsDefaultObject.Settings.GroupTemplateObjects[0];
            var groupTemplate = groupObj as AddressableAssetGroupTemplate;
            if (groupTemplate == null) {
                return null;
            }
            targetGroup = AddressableAssetSettingsDefaultObject.Settings.CreateGroup(newGroupName, false,
                false, true, null, groupTemplate.GetTypes());
            groupTemplate.ApplyToAddressableAssetGroup(targetGroup);
            return targetGroup;
        }

        private void RemoveEntry(AddressableAssetGroup group) {
            var list = new List<string>();
            var removeList = new List<AddressableAssetEntry>();
            foreach (var entry in group.entries) {
                if (GroupFile.Find(o => o.Equals(entry.AssetPath, StringComparison.OrdinalIgnoreCase)) == null) {
                    removeList.Add(entry);
                    continue;
                }
                if (!entry.address.Equals(GetResourceName(entry.AssetPath))) {
                    removeList.Add(entry);
                    continue;
                }
                if (string.IsNullOrEmpty(entry.AssetPath) || !File.Exists(entry.AssetPath)) {
                    removeList.Add(entry);
                    continue;
                }
                if (!list.Contains(entry.address)) {
                    list.Add(entry.address);
                } else {
                    removeList.Add(entry);
                }
            }
            if (removeList.Count <= 0) {
                return;
            }
            for (var i = removeList.Count - 1; i >= 0; i--) {
                group.RemoveAssetEntry(removeList[i]);
            }
        }

        // 依赖资源文件夹分组用这个
        private void CreateAddressableGroupWithDirectory(string dir, bool withChildDir = true) {
            if (!CollectGroupFile(dir, withChildDir)) {
                return;
            }
            CreateAddressableGroupFiles(dir, GroupFile);
        }

        //复合资源（Prefab,Atlas,Scene）用这个分组
        private void CreateAddressableGroup(string dir) {
            if (!CollectGroupFile(dir,true)) {
                return;
            }
            foreach (var file in GroupFile) {
                CreateAddressableSingleFile(dir,file);
            }
        }

        private void CreateAddressableSingleFile(string groupTitle, string file) {
            var groupName = groupTitle + "/" + Path.GetFileNameWithoutExtension(file);
            CreateAddressableGroupFiles(groupName, new List<string> {file});
        }

        private void CreateAddressableGroupWithEachDirectory(string dir) {
            if (string.IsNullOrEmpty(dir)) {
                return;
            }

            var targetDir = Application.dataPath + "/" + dir;
            if (!Directory.Exists(targetDir)) {
                return;
            }

            CreateAddressableGroupWithDirectory(dir,false);
            var dirArray = Directory.GetDirectories(targetDir,"*",SearchOption.AllDirectories);
            foreach (var child in dirArray) {
                var childDir = child.Replace(Application.dataPath + "/", "").Replace("\\", "/");
                CreateAddressableGroupWithDirectory(childDir,false);
            }
        }

        protected void CreateGroup() {
            foreach (var group in _groupDir) {
                CreateAddressableGroupWithDirectory(group);
            }
            foreach (var group in _groupFile) {
                CreateAddressableGroup(group);
            }
            foreach (var group in _groupEach) {
                CreateAddressableGroupWithEachDirectory(group);
            }
        }

        protected void ClearCachedGroup() {
            _groupDir.Clear();
            _groupFile.Clear();
        }

        protected static void DeleteGroupByPrefix(string prefix) {
            var setting = AddressableAssetSettingsDefaultObject.Settings;
            var removeGroups = setting.groups.FindAll(g => g.Name.StartsWith(prefix));
            foreach (var removeGroup in removeGroups) {
                setting.RemoveGroup(removeGroup);
            }
        }

    }
}
