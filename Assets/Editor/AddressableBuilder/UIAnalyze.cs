using System.IO;
using UnityEditor;
namespace TCG.Editor {
    [AddressableGroup(EBuildType.UI)]
    public class UIAnalyze : GroupAnalyzeBase<UIAnalyze> {
        private const string RawTexture = "Art/2D/RawTexture";               // 背景大图片
        private const string UIPrefab = "Art/2D/Objects/UI";                    //UI
        private const string CardPrefab = "Art/2D/Objects/Card";
        private const string CommonPrefab = "Art/2D/Objects/Common";
        private const string FightPrefab = "Art/2D/Objects/Fight";
        private const string SpriteAtlas = "Art/2D/SpriteAtlas";                 // 图集

        public override void AnalyzeGroup() {
            EditorSettings.spritePackerMode = SpritePackerMode.AlwaysOnAtlas;
            AssetImporter.TextureAssetImporter.CheckSpriteSpriteAtlas();
            AddGroupFile(SpriteAtlas);
            AddGroupDir(RawTexture);
            AddGroupFile(UIPrefab);
            AddGroupFile(CardPrefab);
            AddGroupFile(CommonPrefab);
            AddGroupFile(FightPrefab);
            CreateGroup();
        }

        protected override string GetGroupName(string groupName) {
            var newGroupName = groupName.Replace("\\", "/");
            if (newGroupName.Contains(RawTexture + "/")) {
                newGroupName = DependPrefix + Path.GetFileNameWithoutExtension(newGroupName);
                return newGroupName;
            }
            newGroupName = newGroupName.Replace("/", "_");
            newGroupName = newGroupName.Replace("Art_2D_Objects_UI", "UI_Prefab");
            newGroupName = newGroupName.Replace("Art_2D_Objects_Card", "Card_Prefab");
            newGroupName = newGroupName.Replace("Art_2D_Objects_Common", "Common_Prefab");
            newGroupName = newGroupName.Replace("Art_2D_Objects_Fight", "Fight_Prefab");
            newGroupName = newGroupName.Replace("Art_2D_SpriteAtlas", "UI_SpriteAtlas");
            newGroupName = newGroupName.Replace("Art_2D_RawTexture", "UI_RawTexture");

            return newGroupName;
        }

        public override void DeleteGroup() {
            DeleteGroupByPrefix("UI_RawTexture");
            DeleteGroupByPrefix("UI_Prefab");
            DeleteGroupByPrefix("Card_Prefab");
            DeleteGroupByPrefix("Fight_Prefab");
            DeleteGroupByPrefix("Common_Prefab");
            DeleteGroupByPrefix("UI_SpriteAtlas");
        }
    }
}
