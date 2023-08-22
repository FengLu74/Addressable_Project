namespace TCG.Editor {
    public enum EnumAssetBundleType {
        // ReSharper disable UnusedMember.Global
        UI = 1 << 0,                   //ui
        Texture = 1 << 1,         //贴图
        Effect = 1 << 2,          //特效
        Scene = 1 << 3,           //场景
        Sound = 1 << 4,           //音效
        BundleText = 1 << 5,      //lua脚本
        Shader = 1 << 6,          //shader
        Customize = 1 << 7,      //自定义
        DesignData = 1 << 8,     //策划数据相关
        ServerPrefab = 1 << 9,        //服务器相关prefab

        Server = ServerPrefab | BundleText,
        All = 0x1FFFF,
        AllExceptShader = All ^ Shader
        // ReSharper restore UnusedMember.Global
    }
}
