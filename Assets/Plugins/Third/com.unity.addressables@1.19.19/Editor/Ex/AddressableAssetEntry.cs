using UnityEngine;
namespace UnityEditor.AddressableAssets.Settings {
    public partial class AddressableAssetEntry {
        public AddressableAssetEntry(string guid, string address, AddressableAssetGroup parent)
        {
            if (guid.Length > 0 && address.Contains("[") && address.Contains("]"))
                Debug.LogErrorFormat("Address '{0}' cannot contain '[ ]'.", address);
            m_GUID = guid;
            m_Address = address;
            m_ReadOnly = false;
            parentGroup = parent;
            IsInResources = false;
            IsInSceneList = false;
        }
    }
}
