using System;
namespace TCG.Editor {
    public class AddressableGroupAttribute : Attribute {
        public readonly EBuildType AnalyzeType;
        public AddressableGroupAttribute(EBuildType buildType) => AnalyzeType = buildType;
    }
}
