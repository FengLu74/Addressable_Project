namespace UnityEditor.AddressableAssets.Settings {
    public partial class AddressableAssetGroup {
        public void AddAssetEntryEx(AddressableAssetEntry e) {
            e.IsSubAsset = false;
            e.parentGroup = this;
            m_EntryMap[e.guid] = e;
            m_SerializeEntries = null;
            SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, e, true, true);
        }
    }
}
