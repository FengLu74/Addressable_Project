namespace TCG.Editor {
    [AddressableGroup(EBuildType.Spine)]
    public class SpineAnalyze : GroupAnalyzeBase<SpineAnalyze> {
        private const string SpineBytesPath = "Art/2D/SpineAnimation";
        private const string GroupNamePrefix = "UI_Spine";
        public override void AnalyzeGroup() {
            AddGroupEachDir(SpineBytesPath);
            CreateGroup();
        }

        protected override string GetGroupName(string groupName) {
            var newGroupName = groupName.Replace("\\", "/").Replace("/", "_");
            return newGroupName.Replace("Art_2D_SpineAnimation_UI", GroupNamePrefix);
        }

        public override void DeleteGroup() => DeleteGroupByPrefix(GroupNamePrefix);
    }
}
