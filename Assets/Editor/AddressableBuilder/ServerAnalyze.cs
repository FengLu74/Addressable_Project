
namespace TCG.Editor {
    [AddressableGroup(EBuildType.Server)]
    public class ServerAnalyze : GroupAnalyzeBase<ServerAnalyze> {
        private const string CardPrefab = "Art/Server/Objects";

        public override void AnalyzeGroup() {
            AddGroupFile(CardPrefab);
            CreateGroup();
        }

        protected override string GetGroupName(string groupName) {
            var newGroupName = groupName.Replace("\\", "/");
            newGroupName = newGroupName.Replace("/", "_");
            newGroupName = newGroupName.Replace("Art_Server_Objects", "Server_Prefab");

            return newGroupName;
        }

        public override void DeleteGroup() => DeleteGroupByPrefix("Server_Prefab");
    }
}
