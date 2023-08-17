using System.Collections.Generic;
using System.IO;
namespace UnityEditor.AddressableAssets.Settings {
    public partial class AddressableAssetSettings {
        public bool RemoveMissingGroup() {
            if (RemoveMissingGroupReferences()) {
                SetDirty(ModificationEvent.GroupRemoved, null, true, true);
                return true;
            }

            return false;
        }

        public void RemoveMissingEntry() {
            var removeList = new List<AddressableAssetEntry>();
            for (var i = 0; i < groups.Count; i++) {
                var g = groups[i];
                if (g == null)
                    continue;

                foreach (var entry in g.entries) {
                    if (!File.Exists(entry.AssetPath)) {
                        removeList.Add(entry);
                    }
                }

                foreach (var entry in removeList) {
                    g.RemoveAssetEntry(entry);
                }
                removeList.Clear();
            }
        }
    }
}
