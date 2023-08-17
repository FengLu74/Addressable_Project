using UnityEditor;
namespace TCG.Editor.AssetImporter {
    public class AssetImporter : AssetPostprocessor {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths) {
            TextureAssetImporter.OnActionOnPostprocess(importedAssets, deletedAssets, movedAssets);
        }
    }
}
