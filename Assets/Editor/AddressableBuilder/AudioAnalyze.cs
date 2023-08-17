namespace TCG.Editor {
    [AddressableGroup(EBuildType.Audio)]
    public class AudioAnalyze : GroupAnalyzeBase<AudioAnalyze> {
        private const string AudioBytesPath = "Art/Audio";
        private const string GroupNamePrefix = "Audio";
        public override void AnalyzeGroup() {
            AddGroupEachDir(AudioBytesPath);
            CreateGroup();
        }

        protected override string GetGroupName(string groupName) {
            var newGroupName = groupName.Replace("\\", "/").Replace("/", "_");
            return newGroupName.Replace("Art_Audio_UI", GroupNamePrefix);
        }

        public override void DeleteGroup() => DeleteGroupByPrefix(GroupNamePrefix);
    }
}
