namespace TCG.Editor {
    [AddressableGroup(EBuildType.DesignData)]
    public class DesignBytesAnalyze : GroupAnalyzeBase<DesignBytesAnalyze> {
        private const string DesignBytesPath = "Design/Data";
        private const string GroupNamePrefix = "Design_Data";
        public override void AnalyzeGroup() {
            AddGroupDir(DesignBytesPath);
            CreateGroup();
        }

        protected override string GetResourceName(string resName) {
            var newResName = resName.Replace("\\", "/").
                Replace("Assets/Design/Data/", "").
                Replace(".bytes", "");
            return newResName.ToLower();
        }

        public override void DeleteGroup() => DeleteGroupByPrefix(GroupNamePrefix);
    }
}
