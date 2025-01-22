#if TOOLS
using Godot;

[Tool]
public partial class NetfoxSharpPlugin : EditorPlugin
{
    private static readonly string
        Node = "Node",

        RootPath = "res://addons/netfox_sharp/",
        NodePath = "nodes/",
        IconPath = "icons/",
        HideGDScriptNodes = "netfox/sharp/HideGDScriptNodes",
        ClearSettings = "netfox/general/clear_settings";

    private static readonly string[] gdNodes = new[]
    {
        "RollbackSynchronizer",
        "StateSynchronizer",
        "TickInterpolator"
    };

    private static readonly NetfoxNodeData[] nodes = new NetfoxNodeData[]
    {
        new("RollbackSynchronizer", Node),
        new("StateSynchronizer", Node),
        new("TickInterpolator", Node)
    };

    private static readonly NetfoxSettingData[] settings = new NetfoxSettingData[]
    {
        new(HideGDScriptNodes, true, true)
    };

    public override void _EnterTree()
    {
        foreach (NetfoxNodeData node in nodes)
        {
            AddCustomType($"{node.NodeName}Sharp", node.NodeType,
                GD.Load<Script>($"{RootPath}{NodePath}{node.NodeName}.cs"),
                GD.Load<Texture2D>($"{RootPath}{IconPath}{node.NodeName}.svg"));
        }

        foreach (NetfoxSettingData setting in settings)
        {
            if (ProjectSettings.HasSetting(setting.SettingName))
                continue;

            ProjectSettings.SetSetting(setting.SettingName, setting.DefaultValue);
            ProjectSettings.SetInitialValue(setting.SettingName, setting.DefaultValue);
            ProjectSettings.SetAsBasic(setting.SettingName, setting.IsBasic);
        }
    }

    public override void _ExitTree()
    {
        foreach (NetfoxNodeData node in nodes)
            RemoveCustomType($"{node.NodeName}Sharp");

        if ((bool)ProjectSettings.GetSetting(ClearSettings, false))
            foreach (NetfoxSettingData setting in settings)
                ProjectSettings.SetSetting(setting.SettingName, new());
    }

    class NetfoxNodeData
    {
        public readonly string NodeName;
        public readonly string NodeType;

        public NetfoxNodeData(string nodeName, string nodeType)
        {
            NodeName = nodeName;
            NodeType = nodeType;
        }
    }

    class NetfoxSettingData
    {
        public readonly string SettingName;
        public readonly Variant DefaultValue;
        public readonly bool IsBasic;

        public NetfoxSettingData(string settingName, Variant defaultValue, bool isBasic)
        {
            SettingName = settingName;
            DefaultValue = defaultValue;
            IsBasic = isBasic;
        }
    }
}
#endif