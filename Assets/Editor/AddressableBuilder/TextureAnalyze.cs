namespace TCG.Editor {
    [AddressableGroup(EBuildType.Texture)]
    public class TextureAnalyze : GroupAnalyzeBase<TextureAnalyze>
    {
        private const string UiTexture = "Art/2D/UITexture";                           // 图片文理
        private const string GroupNamePrefix = "UI_Texture_";

        public override void AnalyzeGroup()
        {
            AddGroupEachDir(UiTexture);
            CreateGroup();
        }

        protected override string GetLabel() => "preload";

        protected override string GetGroupName(string groupName) {
            var newGroupName = groupName.Replace("\\", "/").Replace("/", "_");
            return newGroupName.Replace("Art_2D_UITexture_", GroupNamePrefix);
        }

        public override void DeleteGroup() => DeleteGroupByPrefix(GroupNamePrefix);
    }
}
