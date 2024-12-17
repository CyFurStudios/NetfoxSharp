#if TOOLS
using Godot;
using System;

[Tool]
public partial class NetfoxSharp : EditorPlugin
{
    private static readonly string
        Node = "Node",

        RootPath = "res://addons/netfox_sharp/",
        NodePath = "nodes/",
        IconPath = "icons/",

        HideGDScriptNodes = "netfox/sharp/HideGDScriptNodes";

    private static readonly string[] gdNodes = new[]
    {
        "RollbackSynchronizer",
        "StateSynchronizer",
        "TickInterpolator"
    };

    private static readonly Tuple<string, string>[] nodes = new Tuple<string, string>[]
    {
        new("RollbackSynchronizer", Node),
        new("StateSynchronizer", Node),
        new("TickInterpolator", Node)
    };

    // Tuple contains
    // string: setting name
    // Variant: default value
    // bool: true if the setting is basic
    public static readonly Tuple<string, Variant, bool>[] settings = new Tuple<string, Variant, bool>[]
    {
        new(HideGDScriptNodes, true, true)
    };

    public override void _EnterTree()
    {
        foreach (Tuple<string, string> node in nodes)
        {
            AddCustomType($"{node.Item1}CS", node.Item2,
                GD.Load<Script>($"{RootPath}{NodePath}{node.Item1}.cs"),
                GD.Load<Texture2D>($"{RootPath}{IconPath}{node.Item1}.svg"));
        }

        foreach (Tuple<string, Variant, bool> setting in settings)
        {
            if (ProjectSettings.HasSetting(setting.Item1))
                continue;

            ProjectSettings.SetSetting(setting.Item1, setting.Item2);
            ProjectSettings.SetInitialValue(setting.Item1, setting.Item2);
            ProjectSettings.SetAsBasic(setting.Item1, setting.Item3);
        }

        CallDeferred(MethodName.CheckNetfoxGd);
    }

    void CheckNetfoxGd()
    {
        // TODO: Currently doesn't work correctly due to class_name in GDScript.
        if ((bool)ProjectSettings.GetSetting(HideGDScriptNodes))
            foreach (string node in gdNodes)
                RemoveCustomType(node);
    }

    public override void _ExitTree()
    {
        foreach (Tuple<string, string> node in nodes)
            RemoveCustomType($"{node.Item1}CS");

        if ((bool)ProjectSettings.GetSetting("netfox/general/clear_settings", false))
            foreach (Tuple<string, Variant, bool> setting in settings)
                ProjectSettings.SetSetting(setting.Item1, new());
    }
}
#endif
